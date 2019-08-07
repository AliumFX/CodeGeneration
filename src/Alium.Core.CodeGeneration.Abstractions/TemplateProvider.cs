// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.CodeGeneration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Provides services for reading templates.
    /// </summary>
    public class TemplateProvider
    {
        private readonly IFileProvider _fileProvider;

        /// <summary>
        /// Initialises a new instance of <see cref="TemplateProvider"/>.
        /// </summary>
        /// <param name="fileProvider">The file provider.</param>
        public TemplateProvider(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        /// <summary>
        /// Reads the given template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <returns>The template content.</returns>
        public async Task<string> ReadTemplate(string name, object templateData = null)
        {
            var file = _fileProvider.GetFileInfo($"{name}.cstemplate");
            if (file is null)
            {
                throw new ArgumentException($"There is not code template named '{name}'");
            }


            using (var stream = file.CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                string content = await reader.ReadToEndAsync();
                if (content is object && templateData is object)
                {
                    return MergeTemplateData(content, templateData);
                }

                return content;
            }
        }

        /// <summary>
        /// Creates a template provider using an embedded file provider for the given assembly.
        /// </summary>
        /// <param name="assembly">The source assembly.</param>
        /// <param name="namespace">The base namespace.</param>
        /// <returns>The template provider.</returns>
        public static TemplateProvider FromAssembly(Assembly assembly, string @namespace)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var fileProvider = new EmbeddedFileProvider(assembly, $"{@namespace}.templates");

            return new TemplateProvider(fileProvider);
        }

        private string MergeTemplateData(string content, object templateData)
        {
            var props = templateData.GetType()
                .GetRuntimeProperties()
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var regex = new Regex(@"(?:TEMPLATE_DATA_)(.*?)(?:__)", RegexOptions.IgnoreCase);

            return regex.Replace(content, m =>
            {
                if (props.TryGetValue(m.Groups[1].Value, out var prop)
                    && prop.GetValue(templateData) is object value)
                {
                    return value.ToString();
                }

                throw new InvalidOperationException($"Unable to match template key {m.Value} with template data.");
            });
        }
    }
}
