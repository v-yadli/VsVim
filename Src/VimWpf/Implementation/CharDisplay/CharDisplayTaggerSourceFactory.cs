﻿using EditorUtils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;

namespace Vim.UI.Wpf.Implementation.CharDisplay
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(VimConstants.AnyContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class CharDisplayTaggerSourceFactory : IViewTaggerProvider
    {
        private readonly object _key = new object();
        private readonly IEditorFormatMapService _editorFormatMapService;
        private readonly IControlCharUtil _controlCharUtil;
        private readonly IVim _vim;

        [ImportingConstructor]
        internal CharDisplayTaggerSourceFactory(IVim vim, IEditorFormatMapService editorFormatMapService, IControlCharUtil controlCharUtil)
        {
            _editorFormatMapService = editorFormatMapService;
            _vim = vim;
            _controlCharUtil = controlCharUtil;
        }

        private CharDisplayTaggerSource CreateCharDisplayTaggerSource(ITextView textView)
        {
            var editorFormatMap = _editorFormatMapService.GetEditorFormatMap(textView);
            return new CharDisplayTaggerSource(textView, editorFormatMap, _controlCharUtil);
        }

        #region IViewTaggerProvider

        ITagger<T> IViewTaggerProvider.CreateTagger<T>(ITextView textView, ITextBuffer textBuffer)
        {
            if (textView.TextBuffer != textBuffer || !_vim.VimHost.ShouldCreateVimBuffer(textView))
            {
                return null;
            }

            return EditorUtilsFactory.CreateTagger(
                textView.Properties,
                _key,
                () => CreateCharDisplayTaggerSource(textView)) as ITagger<T>;
        }

        #endregion
    }
}
