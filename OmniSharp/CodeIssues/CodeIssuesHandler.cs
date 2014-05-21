﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using OmniSharp.Common;
using OmniSharp.Parser;
using OmniSharp.Refactoring;
using OmniSharp.Solution;
using OmniSharp.Configuration;

namespace OmniSharp.CodeIssues
{
    public class CodeIssuesHandler
    {
        private readonly BufferParser _bufferParser;
        private readonly OmniSharpConfiguration _config;

        public CodeIssuesHandler(ISolution solution, BufferParser bufferParser)
        {
            _bufferParser = bufferParser;
            _config = ConfigurationLoader.Config;
        }

        public QuickFixResponse GetCodeIssues(Request req)
        {
            var actions = GetContextualCodeActions(req);
            return new QuickFixResponse(actions.Select(a => new QuickFix
                {
                    Column = a.Start.Column,
                    Line = a.Start.Line,
                    FileName = req.FileName,
                    Text = a.Description
                }));
        }

        public RunCodeIssuesResponse FixCodeIssue(Request req)
        {
            var issues = GetContextualCodeActions(req).ToList();

            var issue = issues.FirstOrDefault(i => i.Start.Line == req.Line);
            if(issue == null)
                return new RunCodeIssuesResponse { Text = req.Buffer };

            var context = OmniSharpRefactoringContext.GetContext(_bufferParser, req);
            
            using (var script = new OmniSharpScript(context))
            {
                issue.Actions.FirstOrDefault().Run(script);
            }

            return new RunCodeIssuesResponse {Text = context.Document.Text};
        }

        private IEnumerable<CodeIssue> GetContextualCodeActions(Request req)
        {
            var refactoringContext = OmniSharpRefactoringContext.GetContext(_bufferParser, req);

            var actions = new List<CodeIssue>();
            var providers = new CodeIssueProviders().GetProviders().Where(
                provider => !_config.IgnoredIssueProviders.Contains(provider.GetType().Name));
            foreach (var provider in providers)
            {
                try
                {
                    var codeIssues = provider.GetIssues(refactoringContext);
                    actions.AddRange(codeIssues);
                }
                catch (Exception)
                {
                }
                
            }
            return actions;
        }
    }
}
