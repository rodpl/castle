// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework
{
	using System;

	/// <summary>
	/// Summary description for IResponse.
	/// </summary>
	public interface IResponse
	{
		void AppendHeader(String name, String value);

		System.IO.TextWriter Output { get; }
		
		System.IO.Stream OutputStream { get; }

		void Write(String s);

		void Write(object obj);

		void Write(char ch);

		void Write(char[] buffer, int index, int count);

		void Redirect(String url);

		void Redirect(String url, bool endProcess);

		int StatusCode { get; set; }

		String ContentType { get; set; }
	}
}
