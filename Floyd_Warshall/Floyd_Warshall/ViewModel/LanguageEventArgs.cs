using System;

namespace Floyd_Warshall.ViewModel
{
	public class LanguageEventArgs : EventArgs
    {
        public LanguageEventArgs(string? langCode)
        {
            LangCode = langCode;
        }

        public string? LangCode { get; }
    }
}
