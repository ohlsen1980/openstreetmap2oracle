//OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
//               (*.osm - files) to an oracle database
//-------------------------------------------------------------------------------
//Copyright (C) 2011  Oliver Schöne
//-------------------------------------------------------------------------------
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenStreetMap2Oracle.tools
{
    /// <summary>
    /// Class to parse Strings for a specific Delimiter an tokenize it
    /// </summary>
    public class StringTokenizer
    {
        private int _enumeratorIndex;
        private int _tokenCount;
        private List<string> _tokens;
        private string _source;
        private string _delimiter;

        public List<string> Tokens
        {
            get
            {
                return _tokens;
            }
        }

        /// <summary>
        /// Creates a new instance of StringTokenizer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        public StringTokenizer(string source, string delimiter = " ")
        {
            this._tokens = new List<string>();
            this._source = source;
            this._delimiter = (delimiter.Length > 0) ? (delimiter) : (" ");
        }

        /// <summary>
        /// Constructor for StringTokenizer Class.
        /// </summary>
        /// <param name="source">The Source String.</param>
        /// <param name="delimiter">The Delimiter String as a char[].  Note that this is converted into a single String and
        /// expects Unicode encoded chars.</param>
        public StringTokenizer(string source, char[] delimiter)
            : this(source, new string(delimiter))
        {
        }

        /// <summary>
        /// Constructor for StringTokenizer Class.  The default delimiter of " " (space) is used.
        /// </summary>
        /// <param name="source">The Source String.</param>
        public StringTokenizer(string source)
            : this(source, "")
        {
        }

        /// <summary>
        /// Empty Constructor.  Will create an empty StringTokenizer with no source, no delimiter, and no tokens.
        /// If you want to use this StringTokenizer you will have to call the NewSource(string s) method.  You may
        /// optionally call the NewDelim(string d) or NewDelim(char[] d) methods if you don't with to use the default
        /// delimiter of " " (space).
        /// </summary>
        public StringTokenizer()
            : this("", "")
        {
        }


        /// <summary>
        /// Tokenizes the string with the given delimiter
        /// </summary>
        private void Tokenize()
        {
            this._enumeratorIndex = 0;
            this._tokens.Clear();

            string[] token_array = this.Source.Split(new string[] { this._delimiter }, StringSplitOptions.RemoveEmptyEntries);
            
            this._tokens.AddRange(token_array);
            this._tokenCount = this._tokens.Count;
        }



        [Obsolete("Use Tokenize instead of this old version")]
        private void __deprecated_Tokenize()
        {
          /*  string TempSource = this._source;
            string Tok = "";
            this._tokenCount = 0;
            this.tokens.Clear();
            this._enumeratorIndex = 0;

            if (TempSource.IndexOf(this._delimiter) < 0 && TempSource.Length > 0)
            {
                this._tokenCount = 1;
                this._enumeratorIndex = 0;
                this.tokens.Add(TempSource);
                this.tokens.TrimToSize();
                TempSource = "";
            }
            else if (TempSource.IndexOf(this._delimiter) < 0 && TempSource.Length <= 0)
            {
                this._tokenCount = 0;
                this._enumeratorIndex = 0;
                this.tokens.TrimToSize();
            }
            while (TempSource.IndexOf(this._delimiter) >= 0)
            {
                //Delimiter at beginning of source String.
                if (TempSource.IndexOf(this._delimiter) == 0)
                {
                    if (TempSource.Length > this._delimiter.Length)
                    {
                        TempSource = TempSource.Substring(this._delimiter.Length);
                    }
                    else
                    {
                        TempSource = "";
                    }
                }
                else
                {
                    Tok = TempSource.Substring(0, TempSource.IndexOf(this._delimiter));
                    this.tokens.Add(Tok);
                    if (TempSource.Length > (this._delimiter.Length + Tok.Length))
                    {
                        TempSource = TempSource.Substring(this._delimiter.Length + Tok.Length);
                    }
                    else
                    {
                        TempSource = "";
                    }
                }
            }
            //we may have a string leftover.
            if (TempSource.Length > 0)
            {
                this.tokens.Add(TempSource);
            }
            this.tokens.TrimToSize();
            this._tokenCount = this.tokens.Count; */
        }

        /// <summary>
        /// Method to add or change this Instance's Source string.  The delimiter will
        /// remain the same (either default of " " (space) or whatever you constructed 
        /// this StringTokenizer with or added with NewDelim(string d) or NewDelim(char[] d) ).
        /// </summary>
        /// <param name="newSrc">The new Source String.</param>
        public void NewSource(string newSrc)
        {
            this._source = newSrc;
            this.Tokenize();
        }

        /// <summary>
        /// Method to add or change this Instance's Delimiter string.  The source string
        /// will remain the same (either empty if you used Empty Constructor, or the 
        /// previous value of source from the call to a parameterized constructor or
        /// NewSource(string s)).
        /// </summary>
        /// <param name="newDel">The new Delimiter String.</param>
        public void NewDelim(string newDel)
        {
            this._delimiter = (newDel.Length > 0) ? newDel : " ";
            this.Tokenize();
        }

        /// <summary>
        /// Method to add or change this Instance's Delimiter string.  The source string
        /// will remain the same (either empty if you used Empty Constructor, or the 
        /// previous value of source from the call to a parameterized constructor or
        /// NewSource(string s)).
        /// </summary>
        /// <param name="newDel">The new Delimiter as a char[].  Note that this is converted into a single String and
        /// expects Unicode encoded chars.</param>
        public void NewDelim(char[] newDel)
        {
            this._delimiter = (newDel.Length > 0) ? (new string(newDel)) : " ";
            this.Tokenize();
        }

        /// <summary>
        /// Method to get the number of tokens in this StringTokenizer.
        /// </summary>
        /// <returns>The number of Tokens in the internal ArrayList.</returns>
        public int CountTokens()
        {
            return this._tokens.Count;
        }

        /// <summary>
        /// Method to probe for more tokens.
        /// </summary>
        /// <returns>true if there are more tokens; false otherwise.</returns>
        public bool HasMoreTokens()
        {
            return (this._enumeratorIndex < (this._tokens.Count));
            
            // @note: performance optimized return
            /*
            if (this._enumeratorIndex <= (this._tokens.Count - 1))
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }

        /// <summary>
        /// Method to get the next (string)token of this StringTokenizer.
        /// </summary>
        /// <returns>A string representing the next token; null if no tokens or no more tokens.</returns>
        public string NextToken()
        {
            return (this.HasMoreTokens() ? _tokens[_enumeratorIndex++] : null);
        }

        /// <summary>
        /// Gets the Source string of this Stringtokenizer.
        /// </summary>
        /// <returns>A string representing the current Source.</returns>
        public string Source
        {
            get
            {
                return this._source;
            }
        }

        /// <summary>
        /// Gets the Delimiter string of this StringTokenizer.
        /// </summary>
        /// <returns>A string representing the current Delimiter.</returns>
        public string Delim
        {
            get
            {
                return this._delimiter;
            }
        }

    }
}
