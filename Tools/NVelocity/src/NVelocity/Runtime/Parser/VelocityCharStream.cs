// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.Runtime.Parser
{
	using System;
	using System.IO;

	/// <summary>  NOTE : This class was originally an ASCII_CharStream autogenerated
	/// by Javacc.  It was then modified via changing class name with appropriate
	/// fixes for CTORS, and mods to readChar().
	///
	/// This is safe because we *always* use Reader with this class, and never a
	/// InputStream.  This guarantees that we have a correct stream of 16-bit
	/// chars - all encoding transformations have been done elsewhere, so we
	/// believe that there is no risk in doing this.  Time will tell :)
	/// </summary>
	/// <summary> An implementation of interface CharStream, where the stream is assumed to
	/// contain only ASCII characters (without unicode processing).
	/// </summary>
	public sealed class VelocityCharStream : ICharStream
	{
		public const bool staticFlag = false;
		internal int bufferSize;
		internal int available;
		internal int tokenBegin;
		public int bufferPosition = -1;
		private int[] bufferLine;
		private int[] bufferColumn;

		private int column = 0;
		private int line = 1;

		private bool prevCharIsCR = false;
		private bool prevCharIsLF = false;

		private TextReader inputStream;

		private char[] buffer;
		private int maxNextCharInd = 0;
		private int inBuf = 0;
		private char currentCharacter;
		private bool currentCharacterAvailable = false;

		public VelocityCharStream(TextReader textReader,
		                          int startLine, int startColumn, int bufferSize)
		{
			inputStream = textReader;
			line = startLine;
			column = startColumn - 1;

			available = this.bufferSize = bufferSize;
			buffer = new char[bufferSize];
			bufferLine = new int[bufferSize];
			bufferColumn = new int[bufferSize];
		}

		public VelocityCharStream(TextReader textReader, int startLine, int startColumn)
			: this(textReader, startLine, startColumn, 4096)
		{
		}

		public void ReInit(TextReader textReader, int startLine, int startColumn, int bufferSize)
		{
			inputStream = textReader;
			line = startLine;
			column = startColumn - 1;

			if (buffer == null || bufferSize != buffer.Length)
			{
				available = this.bufferSize = bufferSize;
				buffer = new char[bufferSize];
				bufferLine = new int[bufferSize];
				bufferColumn = new int[bufferSize];
			}
			prevCharIsLF = prevCharIsCR = false;
			tokenBegin = inBuf = maxNextCharInd = 0;
			bufferPosition = -1;
		}

		public void ReInit(TextReader textReader, int startLine, int startColumn)
		{
			ReInit(textReader, startLine, startColumn, 4096);
		}

		public String GetImage()
		{
			if (bufferPosition >= tokenBegin)
			{
				Int32 len = (bufferPosition - tokenBegin + 1) > buffer.Length ? buffer.Length : (bufferPosition - tokenBegin + 1);
				//return new String(buffer, tokenBegin, bufferPosition - tokenBegin + 1);
				return new String(buffer, tokenBegin, len);
			}
			else
			{
				return new String(buffer, tokenBegin, bufferSize - tokenBegin) + new String(buffer, 0, bufferPosition + 1);
			}
		}

		public char[] GetSuffix(int len)
		{
			char[] ret = new char[len];

			if ((bufferPosition + 1) >= len)
				Array.Copy(buffer, bufferPosition - len + 1, ret, 0, len);
			else
			{
				Array.Copy(buffer, bufferSize - (len - bufferPosition - 1), ret, 0, len - bufferPosition - 1);
				Array.Copy(buffer, 0, ret, len - bufferPosition - 1, bufferPosition + 1);
			}

			return ret;
		}

		public void Done()
		{
			buffer = null;
			bufferLine = null;
			bufferColumn = null;
		}

		/// <summary> Method to adjust line and column numbers for the start of a token.<br/>
		/// </summary>
		public void AdjustBeginLineColumn(int newLine, int newCol)
		{
			int start = tokenBegin;
			int len;

			if (bufferPosition >= tokenBegin)
			{
				len = bufferPosition - tokenBegin + inBuf + 1;
			}
			else
			{
				len = bufferSize - tokenBegin + bufferPosition + 1 + inBuf;
			}

			int i = 0;
			int j = 0;
			int k;
			int columnDiff = 0;

			while(i < len && bufferLine[j = start % bufferSize] == bufferLine[k = ++start % bufferSize])
			{
				bufferLine[j] = newLine;
				int nextColDiff = columnDiff + bufferColumn[k] - bufferColumn[j];
				bufferColumn[j] = newCol + columnDiff;
				columnDiff = nextColDiff;
				i++;
			}

			if (i < len)
			{
				bufferLine[j] = newLine++;
				bufferColumn[j] = newCol + columnDiff;

				while(i++ < len)
				{
					if (bufferLine[j = start % bufferSize] != bufferLine[++start % bufferSize])
						bufferLine[j] = newLine++;
					else
						bufferLine[j] = newLine;
				}
			}

			line = bufferLine[j];
			column = bufferColumn[j];
		}

		public int Column
		{
			get { return bufferColumn[bufferPosition]; }
		}

		public int Line
		{
			get { return bufferLine[bufferPosition]; }
		}

		public int EndColumn
		{
			get { return bufferColumn[bufferPosition]; }
		}

		public int EndLine
		{
			get { return bufferLine[bufferPosition]; }
		}

		public int BeginColumn
		{
			get { return bufferColumn[tokenBegin]; }
		}

		public int BeginLine
		{
			get { return bufferLine[tokenBegin]; }
		}

		public char CurrentCharacter
		{
			get
			{
				if (!currentCharacterAvailable)
				{
					throw new InvalidOperationException("CurrentCharacter not available");
				}
				return currentCharacter;
			}
		}

		public bool CurrentCharacterAvailable
		{
			get { return currentCharacterAvailable; }
		}


		private void ExpandBuff(bool wrapAround)
		{
			char[] newBuffer = new char[bufferSize + 2048];
			int[] newBufferLine = new int[bufferSize + 2048];
			int[] newBufferColumn = new int[bufferSize + 2048];

			try
			{
				if (wrapAround)
				{
					Array.Copy(buffer, tokenBegin, newBuffer, 0, bufferSize - tokenBegin);
					Array.Copy(buffer, 0, newBuffer, bufferSize - tokenBegin, bufferPosition);
					buffer = newBuffer;

					Array.Copy(bufferLine, tokenBegin, newBufferLine, 0, bufferSize - tokenBegin);
					Array.Copy(bufferLine, 0, newBufferLine, bufferSize - tokenBegin, bufferPosition);
					bufferLine = newBufferLine;

					Array.Copy(bufferColumn, tokenBegin, newBufferColumn, 0, bufferSize - tokenBegin);
					Array.Copy(bufferColumn, 0, newBufferColumn, bufferSize - tokenBegin, bufferPosition);
					bufferColumn = newBufferColumn;

					maxNextCharInd = (bufferPosition += (bufferSize - tokenBegin));
				}
				else
				{
					Array.Copy(buffer, tokenBegin, newBuffer, 0, bufferSize - tokenBegin);
					buffer = newBuffer;

					Array.Copy(bufferLine, tokenBegin, newBufferLine, 0, bufferSize - tokenBegin);
					bufferLine = newBufferLine;

					Array.Copy(bufferColumn, tokenBegin, newBufferColumn, 0, bufferSize - tokenBegin);
					bufferColumn = newBufferColumn;

					maxNextCharInd = (bufferPosition -= tokenBegin);
				}
			}
			catch(Exception t)
			{
				throw new ApplicationException(t.Message);
			}

			bufferSize += 2048;
			available = bufferSize;
			tokenBegin = 0;
		}

		private bool FillBuff()
		{
			if (maxNextCharInd == available)
			{
				if (available == bufferSize)
				{
					if (tokenBegin > 2048)
					{
						bufferPosition = maxNextCharInd = 0;
						available = tokenBegin;
					}
					else if (tokenBegin < 0)
					{
						bufferPosition = maxNextCharInd = 0;
					}
					else
					{
						ExpandBuff(false);
					}
				}
				else if (available > tokenBegin)
				{
					available = bufferSize;
				}
				else if ((tokenBegin - available) < 2048)
				{
					ExpandBuff(true);
				}
				else
				{
					available = tokenBegin;
				}
			}

			try
			{
				int canRead = inputStream.Read(buffer, maxNextCharInd, available - maxNextCharInd);

				if (canRead <= 0)
				{
					inputStream.Close();
					return EndRead();
				}
				else
				{
					maxNextCharInd += canRead;
					return true;
				}
			}
			catch(Exception)
			{
				return EndRead();
			}
		}


		private bool EndRead()
		{
			--bufferPosition;
			Backup(0);
			if (tokenBegin == -1)
			{
				tokenBegin = bufferPosition;
			}
			return false;
		}


		public bool BeginToken()
		{
			tokenBegin = -1;
			bool success = ReadChar();
			if (success)
			{
				tokenBegin = bufferPosition;
			}

			return success;
		}

		private void UpdateLineColumn()
		{
			column++;

			if (prevCharIsLF)
			{
				prevCharIsLF = false;
				line += (column = 1);
			}
			else if (prevCharIsCR)
			{
				prevCharIsCR = false;
				if (currentCharacter == '\n')
				{
					prevCharIsLF = true;
				}
				else
					line += (column = 1);
			}

			switch(currentCharacter)
			{
				case '\r':
					prevCharIsCR = true;
					break;

				case '\n':
					prevCharIsLF = true;
					break;

				case '\t':
					column--;
					column += (8 - (column & 7));
					break;

				default:
					break;
			}

			bufferLine[bufferPosition] = line;
			bufferColumn[bufferPosition] = column;
		}


		public bool ReadChar()
		{
			if (inBuf > 0)
			{
				--inBuf;

				/*
				*  was : return (char)((char)0xff & buffer[(bufferPosition == bufferSize - 1) ? (bufferPosition = 0) : ++bufferPosition]);
				*/
				currentCharacterAvailable = true;
				currentCharacter = buffer[(bufferPosition == bufferSize - 1) ? (bufferPosition = 0) : ++bufferPosition];
				return true;
			}

			if (++bufferPosition >= maxNextCharInd)
			{
				if (!FillBuff())
				{
					currentCharacterAvailable = false;
					currentCharacter = default(Char);
					return false;
				}
			}

			/*
			*  was : char c = (char)((char)0xff & buffer[bufferPosition]);
			*/
			currentCharacterAvailable = true;
			currentCharacter = buffer[bufferPosition];

			UpdateLineColumn();
			return true;
		}

		public void Backup(int amount)
		{
			inBuf += amount;
			if ((bufferPosition -= amount) < 0)
			{
				bufferPosition += bufferSize;
			}
		}
	}
}