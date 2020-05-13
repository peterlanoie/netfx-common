using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Data;
using Imaging = System.Drawing.Imaging;

namespace Common.Rtf
{
	/// <summary>
	/// The RtfWriter is used to generate RTF documents 'on the fly'.  The methods contained
	/// within the class are used to generate RTF tags, which are in turn interpreted by
	/// RTF readers to generate a Rich Text Document.
	/// </summary>
	/// <remarks>The simplest RTF document must contain the Starting information and Ending 
	/// information.  This is done using the StartDocument() and EndDocument() methods.  
	/// The RTF information and tags created using this class must be concatonated together 
	/// to generate an ASCII text string.  This string can be written to a text file or, 
	/// in the case of a ASP.NET web page, sent directly back to the user using the 
	/// Response header.</remarks>
	/// <example>
	/// <code>
	/// //This is the method for an ASP.NET System.Web.UI.WebControls.Button called btnCreate
	/// //You must use the System.Text namespace for this example
	/// private void btnCreate_Click(object sender, System.EventArgs e)
	/// {
	///     //declare variables
	///     RTFWriter rtf = new RTFWriter();
	///     StringBuilder sbDoc = new StringBuilder();
	///
	///     //start rtf document
	///     sbDoc.Append(rtf.StartDocument(RtfWriter.eFontType.Arial, .5, .5, .5, .5));
	///
	///     //set the font size, type, alignment and indent
	///     sbDoc.Append(rtf.SetFontSize(14) + rtf.SetFontType(RtfWriter.eFontType.Arial));
	///     sbDoc.Append(rtf.SetAlignment(RtfWriter.eAlignment.Left));
	///     sbDoc.Append(rtf.ParagraphIndent(1.0));
	///
	///     //Add some text
	///     sbDoc.Append(rtf.SetFontStyle(RtfWriter.eFontStyle.Bold, RtfWriter.eSetting.On)
	///          + "This is my first sentence (in bold) using the RTF Writer.";
	///     sbDoc.Append(rtf.CarriageReturn() + rtf.SetFontStyle(RtfWriter.eFontStyle.Bold, RtfWriter.eSetting.Off));
	///
	///     //Add some more text
	///     sbDoc.Append(rtf.SetFontSize(10) + rtf.SetFontStyle(RtfWriter.eFontStyle.Italic, RtfWriter.eSetting.On)
	///          + "I am writing in Italics now." + rtf.SetFontStyle(RtfWriter.eFontStyle.Italic, RtfWriter.eSetting.Off);
	///     sbDoc.Append(rtf.CarriageReturn());
	///
	///     //end document
	///     sbDoc.Append(rtf.EndDocument());
	///
	///     //append a header to the response to force a download of the RTF document as
	///     //an attachment
	///     Response.AppendHeader("Content-Disposition", "Attachment;FileName=" +
	///          "HotSheet.rtf");
	///
	///     //set the content type
	///     Response.ContentType = "application/rtf";
	///
	///     //write the file to the Response Header
	///     //to write the StringBuilder text to the filename listed above
	///     Response.Write(sbDoc.ToString());
	///
	///     //stop the rest of the page from being process to prevent sending page HTML
	///     //mixed with the RTF file
	///     try
	///     {
	///          Response.End();
	///     }
	///     catch
	///     {
	///          //Do nothing, just let page finish processing
	///          //and exit for this example
	///     }
	/// }
	///</code>
	///</example>
	public class RtfWriter
	{
		#region Private Member Variables

		private string fontColor;
		private string fontType;
		private string fontSize;
		static char[] hexDigits = {'0', '1', '2', '3', '4', '5', '6', '7',
								'8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
		#endregion
		
		#region Public Enumerators
		
		/// <summary>
		/// Used to turn on/off font style (bold, italic, underline) settings 
		/// </summary>
		public enum eSetting {On = 1, Off = 0};
		/// <summary>
		/// Types of fonts that are available
		/// </summary>
		public enum eFontType {TimesNewRoman, Arial, Symbol};
		/// <summary>
		/// Types of font styles that are available
		/// </summary>
		public enum eFontStyle {Bold, Italic, Underline};
		/// <summary>
		/// Types of font colors that are available
		/// </summary>
		public enum eColor 
		{
			Black = 1, Blue = 2, DarkBlue = 3, Green = 4, 
			DarkGreen = 5, Red = 6, Yellow = 7};
		/// <summary>
		/// Types of paragraph alignments that are available.
		/// </summary>
		public enum eAlignment {Left, Center, Right};
		/// <summary>
		/// Types of lists that are available.
		/// </summary>
		public enum eListType {DecimalNumbering, UppercaseAlpha, LowercaseAlpha}
		/// <summary>
		/// The types of suffix to place after the list character.
		/// </summary>
		public enum eListCharacterSuffix {None, Period, Parentheses}
		#endregion
		
		#region Read-only Properties
		
		/// <summary>
		/// Gets the current font type setting
		/// </summary>
		public string FontType
		{
			get
			{
				return fontType;
			}
		}
		/// <summary>
		/// Gets the current font size (in pts) that is being used
		/// </summary>
		public string FontSize
		{
			get
			{
				return fontSize;
			}
		}
		/// <summary>
		/// Gets the current font color that is being used
		/// </summary>
		public string FontColor
		{
			get	
			{
				return fontColor;
			}
		}
		
		#endregion
		
		#region Private Methods
		/// <summary>
		/// Used internally, converts inches to twips (format used by RTF docs).
		/// 1440 twips = 1 inch.
		/// </summary>
		/// <param name="dInch">Number in inches.</param>
		/// <returns>String containing the double represented as twips.</returns>
		private string InchesToTwips(double dInch)
		{
			string sNum;
			double dNum;

			//check for value of 0(zero)
			if (dInch == 0d)
			{
				return "0";
			}
			else
			{
				//multiply to get twips
				dNum = dInch * 1440D;

				//format string to have no decimal places
				return sNum = String.Format("{0:#####}",dNum);
			}

			
		}
		/// <summary>
		/// Converts inches to pixels.  Uses a default value of 96 pixels = 1 inch
		/// that RTF documents use.
		/// </summary>
		/// <param name="dInch">Number of inches.</param>
		/// <returns>String containing the double represented as pixels.</returns>
		private string InchesToPixels(double dInch)
		{
			//assumes dpi of 96 (default image print quality in rtf)
			string sNum;
			double dNum;

			//check for value of 0(zero)
			if (dInch == 0d)
			{
				return "0";
			}
			else
			{
				//multiply to get twips
				dNum = dInch * 96D;

				//format string to have no decimal places
				return sNum = String.Format("{0:#####}",dNum);
			}


		}
		/// <summary>
		/// Creates required starting information for the RTF document.
		/// </summary>
		/// <param name="sDefFont">Number representing the default font value.</param>
		/// <returns>String containing RTF tags and information.</returns>
		private string fnDocStart(string sDefFont)
		{
			//declare variables
			string sDH;

			//add default attributes and font table to RTF string
			sDH = @"{\rtf1\ansi\ansicpg1252\deff" + sDefFont + @"\deflang1033" +
				@"{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}" +
				@"{\f1\froman\fprq2\fcharset0 Arial;}" +
				@"{\f2\fnil\fcharset2 Symbol;}}";

			//add color table to RTF string
			sDH += @"{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;" +
				@"\red0\green0\blue128;\red0\green255\blue0;" +
				@"\red0\green128\blue0;\red255\green0\blue0;" +
				@"\red255\green255\blue0;}";
			
			//add miscellaneous info (default view)
			sDH += @"\viewkind1\uc1\pard";

			//output results
			return sDH;

		}
		
		/// <summary>
		/// Used internally, return all page margins in Twips (left, right, top, bottom).  Margins
		/// are entered in inches.
		/// </summary>
		/// <param name="Left">Left margin (in inches).</param>
		/// <param name="Right">Right margin (in inches).</param>
		/// <param name="Top">Top margin (in inches).</param>
		/// <param name="Bottom">Bottom margin (in inches).</param>
		/// <returns>String containing the margin info in RTF format.</returns>
		private string fnPageMargins(double Left, double Right, double Top, double Bottom)
		{
			string sPgMarg = "";
			
			//			convert inches to twips, concat together to form string
			sPgMarg += @"\margl" + InchesToTwips(Left);
			sPgMarg += @"\margr" + InchesToTwips(Right);
			sPgMarg += @"\margt" + InchesToTwips(Top);
			sPgMarg += @"\margb" + InchesToTwips(Bottom);

			//			output string
			return sPgMarg;
		}
		
		/// <summary>
		/// Resizes image size to be within the maximum allowed.
		/// </summary>
		/// <param name="imageSize">Current image size.</param>
		/// <param name="MaxW_MaxH">Maximum image size.</param>
		/// <returns>System.Drawing.Size object containing new image size.</returns>
		private Size fnScaleImage(Size imageSize, Size MaxW_MaxH)
		{
			double multBy= 1.01;
			double w= imageSize.Width;   double h= imageSize.Height;

//			scale up
//			while(w < MaxW_MaxH.Width && h < MaxW_MaxH.Height)
//			{
//				w= imageSize.Width*multBy;
//				h= imageSize.Height*multBy;
//				multBy= multBy+.001;
//			}

			//scale down
			while(w > MaxW_MaxH.Width || h > MaxW_MaxH.Height)
			{
				multBy= multBy-.001;
				w= imageSize.Width*multBy;
				h= imageSize.Height*multBy;
			}

			if(imageSize.Width < 1)
				imageSize=new Size(imageSize.Width+-imageSize.Width+1, imageSize.Height-imageSize.Width-1);
			if(imageSize.Height < 1)
				imageSize=new Size(imageSize.Width-imageSize.Height-1, imageSize.Height+-imageSize.Height+1);

			imageSize= new Size(Convert.ToInt32(w), Convert.ToInt32(h));
			return imageSize;

		}

		/// <summary>
		/// Gets the incoding info for the corresponding MIME type.
		/// </summary>
		/// <param name="mimeType">MIME Type (i.e. "image/jpeg").</param>
		/// <returns>System.Drawing.Imaging.ImageCodecInfo</returns>
		private static Imaging.ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			Imaging.ImageCodecInfo[] encoders = Imaging.ImageCodecInfo.GetImageEncoders();
			for(j = 0; j < encoders.Length; ++j)
			{
				if(encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		/// <summary>
		/// Converts binary data to a hex string.
		/// </summary>
		/// <param name="ImageBytes">Byte array containing the binary data to be converted.</param>
		/// <returns>String containing the hexidecimal representation of the binary data.</returns>
		private string BinaryToHex(byte[] ImageBytes)
		{
			//create new char array twice as big as old array
			//2 hex characters = 1 byte (8 bits) of data
			//ie: FF = 1111 1111
			char[] chars = new char[ImageBytes.Length * 2];
			
			//loop to convert each byte to hex equivalent
			for (int i = 0; i < ImageBytes.Length; i++) 
			{
				int b = ImageBytes[i];
				chars[i * 2] = hexDigits[b >> 4];
				chars[i * 2 + 1] = hexDigits[b & 0xF];
			}
			return new string(chars);
		}
		/// <summary>
		/// Removes invalid RTF characters and replaces with the valid RTF character
		/// string
		/// </summary>
		/// <param name="sb">StringBuilder object containing the string to be analyzed</param>
		/// <returns>StringBuilder object containing the valid string</returns>
		private StringBuilder fnRemoveSpecialChar(StringBuilder sb)
		{			
			sb.Replace(Convert.ToString('\u005C'), @"\'5C"); //backslash \
			sb.Replace(Convert.ToString('\u007B'), @"\'7B"); //left curly bracket {
			sb.Replace(Convert.ToString('\u007D'), @"\'7D"); //right curly bracket }
			sb.Replace(Convert.ToString('\u2018'), @"\'91"); //left-single qoute
			sb.Replace(Convert.ToString('\u2019'), @"\'92"); //right-single qoute
			sb.Replace(Convert.ToString('\u201C'), @"\'93"); //left-double qoute
			sb.Replace(Convert.ToString('\u201D'), @"\'94"); //right-double qoute
			sb.Replace(Convert.ToString('\u00BC'), @"\'BC"); //1/4 symbol
			sb.Replace(Convert.ToString('\u00BD'), @"\'BD"); //1/2 symbol
			sb.Replace(Convert.ToString('\u00BE'), @"\'BE"); //3/4 symbol
			sb.Replace(Convert.ToString((char)13), @"\par "); //CarriageReturn symbol
			sb.Replace(Convert.ToString((char)10), @""); //LineFeed symbol
			sb.Replace(Convert.ToString('\u2122'), @"\'99"); //TM symbol
			sb.Replace(Convert.ToString('\u00A9'), @"\'A9"); //Copyright symbol
			sb.Replace(Convert.ToString('\u00AE'), @"\'AE"); //Registered symbol
			sb.Replace(Convert.ToString('\u2013'), @"\'96"); //EN DASH -

			return sb;
		}
		/// <summary>
		/// Removes invalid RTF characters and replaces with the valid RTF character
		/// string
		/// </summary>
		/// <param name="sb">String containing the text to be analyzed</param>
		/// <returns>String containing the valid text</returns>
		private string fnRemoveSpecialChar(string s)
		{
			//declare objects
			StringBuilder sb = new StringBuilder(s);
			
			//call overloaded method to perform functions
			sb = fnRemoveSpecialChar(sb);
			return sb.ToString();
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Inserts a new section into the RTF document.
		/// </summary>
		/// <returns>String containing the RTF new section tag.</returns>
		public string NewSection()
			//insert a section break into the rtf doc
		{
			return @"\sect ";
		}

		/// <summary>
		/// Inserts a page break into the RTF document.
		/// </summary>
		/// <returns>String containing the RTF new page tag.</returns>
		public string NewPage()
			//insert a page break into the rtf doc
		{
			return @"\page ";
		}

		/// <summary>
		/// Returns the corresponding font from the RTF docs font table.
		/// </summary>
		/// <param name="ft">Choosen from the eFontType enumerator.</param>
		/// <returns>String containing the RTF font type tag.</returns>
		public string SetFontType(RtfWriter.eFontType ft)
		{
			string sFT = "";

			//choose which string to output
			switch((int)ft)
			{
				case 0:
					sFT = @"\f0";
					break;
				case 1:
					sFT = @"\f1";
					break;
				case 2:
					sFT = @"\f2";
					break;
				default:
					sFT =  @"\f0";
					break;
			}
			//store font property
			fontType = sFT;

			//output string
			return sFT + " ";
		}
		/// <summary>
		/// Sets the font size using the eFontType enumerator.
		/// </summary>
		/// <param name="size">Integer representing the size in points (minimum 6, maximum 40).</param>
		/// <returns>String containing the RTF font size tag.</returns>
		public string SetFontSize(int size)
		{
			//declare variables
			int iNum;
			int iMin = 12;
			int iMax = 80;
			string sFS;

			//get the absolute value of s (force font to be positive), and double to get
			//point size
			iNum = 2*(Math.Abs(size));

			//don't let font be smaller than 6(pt 12) or larger than 40(pt 80)
			if (iNum < 12)
			{
				sFS = @"\fs" + iMin.ToString();
			}
			else if (iNum > 80)
			{
				sFS = @"\fs" + iMax.ToString();
			}
			else
			{
				sFS = @"\fs" + iNum.ToString();
			}
			//store font size property
			fontSize = sFS;

			//output string
			return sFS + " ";
		}

		/// <summary>
		/// Sets the font color using the eColor enumerator.
		/// </summary>
		/// <param name="fc">Choosen from the eColor enumerator.</param>
		/// <returns>String containing the RTF font color tag.</returns>
		public string SetFontColor(RtfWriter.eColor fc)
		{
			string sFC = "";

			//			choose which string to output
			switch((int)fc)
			{
				case 0:
					sFC = @"\cf0";
					break;
				case 1:
					sFC = @"\cf1";
					break;
				case 2:
					sFC = @"\cf2";
					break;
				case 3:
					sFC = @"\cf3";
					break;
				case 4:
					sFC = @"\cf4";
					break;
				case 5:
					sFC = @"\cf5";
					break;
				case 6:
					sFC = @"\cf6";
					break;
				case 7:
					sFC = @"\cf7";
					break;
				default:
					sFC =  @"\cf0";
					break;
			}
			//			store font color property
			fontColor = sFC;

			//			output string
			return sFC + " ";

		}
		
		/// <summary>
		/// Used to set text alignment to left, center or right.
		/// </summary>
		/// <param name="a">Choosen from the eAlignment enumerator.</param>
		/// <returns>String containing the RTF alignment tag.</returns>
		public string SetAlignment(RtfWriter.eAlignment a)
		{
			//set alignment: 0 = left align, 1 = center, 2 = right
			switch((int)a)
			{
				case 0: 
					return @"\ql ";
				case 1:
					return @"\qc ";
				case 2:
					return @"\qr ";
				default:
					return @"\ql ";
			}
		}

		/// <summary>
		/// Used to set text font style to bold, italic or underline
		/// </summary>
		/// <param name="fs">The type of style that you would like to set, 
		/// choosen from the eFontStyle enumerator</param>
		/// <param name="s">Turn the style on/off using the eSetting enumerator</param>
		/// <returns>String containing the RTF tag to turn on/off font style.</returns>
		public string SetFontStyle(RtfWriter.eFontStyle fs, RtfWriter.eSetting s)
		{
			int style = (int)fs;
			int setting = (int)s;

			//set style: 0 = bold, 1 = italic, 2 = underline, setting 1 = on
			//default turns all styles off
			switch(style)
			{
				case 0:
					if (setting == 1)
					{
						return @"\b ";
					}
					else
					{
						return @"\b0 ";
					}
				case 1:
					if (setting == 1)
					{
						return @"\i ";
					}
					else
					{
						return @"\i0 ";
					}
				case 2:
					if (setting == 1)
					{
						return @"\ul ";
					}
					else
					{
						return @"\ulnone ";
					}
				default:
					//All styles off	
					return @"\b0\i0\ulnone ";
			}
		}
			
		/// <summary>
		/// Insert a single carriage return into the RTF document
		/// </summary>
		/// <returns>String containing the RTF carriage return tag.</returns>
		public string CarriageReturn()
		{
			return @"\par ";
		}

		/// <summary>
		/// Insert the specified number of carriage returns into the RTF document
		/// </summary>
		/// <param name="NumberOfLines">Number of carriage returns to insert (max 20)</param>
		/// <returns>String containing up to 20 RTF carriage return tags.</returns>
		public string CarriageReturn(int NumberOfLines)
		{
			//initialize variables
			int iMax = 1;
			string sCR = "";

			//do not allow more than 20 carriage returns in a row
			if (NumberOfLines > 20)
			{
				iMax = 20;
			}
			else if(NumberOfLines == 0)
			{
				iMax = 1;
			}
			else
			{
				iMax = NumberOfLines;
			}

			//loop to concat all strings together
			for (int iCount = 0; iCount < iMax; iCount ++)
			{
				sCR += @"\par"; 
			}

			//			output string
			return sCR + " ";
		}
	
		/// <summary>
		/// Set paragraph line indents.  Allow first line to have a different indent than
		/// the rest of the paragraph.  The first line indent can be positive or negative
		/// </summary>
		/// <param name="LeftIndent">Indent amount (in inches)</param>
		/// <param name="FirstLine">First line indent amount (in inches).  Can be a
		/// positive or negative number</param>
		/// <returns>String containg RTF tags to set the paragraph indent.</returns>
		public string ParagraphIndent(double LeftIndent, double FirstLine)
		{
			string sIndent;

			//convert inches to twips, create string
			sIndent = @"\fi" + InchesToTwips(FirstLine);
			sIndent += @"\li" + InchesToTwips(LeftIndent);
			return sIndent + " ";
		}
		
		/// <summary>
		/// Set paragraph line indents.  All lines (including the first line) start at the
		/// same indent.
		/// </summary>
		/// <param name="LeftIndent">Indent amount (in inches)</param>
		/// <returns>String containg RTF tags to set the paragraph indent.</returns>
		public string ParagraphIndent(double LeftIndent)
		{
			string sIndent;

			//convert inches to twips, create string
			sIndent = @"\fi0";// + InchesToTwips(LeftIndent);
			sIndent += @"\li" + InchesToTwips(LeftIndent);
			return sIndent + " ";
		}
		/// <summary>
		/// Set a single tab position.  
		/// </summary>
		/// <param name="Tab">Tab position (in inches)</param>
		/// <returns>String containg RTF tags to set the tab location.</returns>
		public string SetTabPosition(double Tab)
		{
			return @"\tx" + InchesToTwips(Tab) + " ";
		}
		/// <summary>
		/// Set two tab positions.
		/// </summary>
		/// <param name="Tab1">First tab position (in inches)</param>
		/// <param name="Tab2">Second tab position (in inches)</param>
		/// <returns>String containg RTF tags to set the tab locations.</returns>
		public string SetTabPosition(double Tab1, double Tab2)
		{
			return @"\tx" + InchesToTwips(Tab1) + @"\tx" + InchesToTwips(Tab2) + " ";
		}
		/// <summary>
		/// Set three tab positions.  Tab positions are measured in inches.
		/// </summary>
		/// <param name="Tab1">First tab position (in inches)</param>
		/// <param name="Tab2">Second tab position (in inches)</param>
		/// <param name="Tab3">Third tab position (in inches)</param>
		/// <returns>String containg RTF tags to set the tab locations.</returns>
		public string SetTabPosition(double Tab1, double Tab2, double Tab3)
		{
			string sTab;
			
			//concat tab positions into one string
			sTab =  @"\tx" + InchesToTwips(Tab1);
			sTab += @"\tx" + InchesToTwips(Tab2);
			sTab += @"\tx" + InchesToTwips(Tab3);

			//output result
			return sTab + " ";
		}
		/// <summary>
		/// Set four tab positions.  Tab positions are measured in inches.
		/// </summary>
		/// <param name="Tab1">First tab position (in inches)</param>
		/// <param name="Tab2">Second tab position (in inches)</param>
		/// <param name="Tab3">Third tab position (in inches)</param>
		/// <param name="Tab4">Forth tab position (in inches)</param>
		/// <returns>String containg RTF tags to set the tab locations.</returns>
		public string SetTabPosition(double Tab1, double Tab2, double Tab3, double Tab4)
		{
			string sTab;
			
			//concat tab positions into one string
			sTab =  @"\tx" + InchesToTwips(Tab1);
			sTab += @"\tx" + InchesToTwips(Tab2);
			sTab += @"\tx" + InchesToTwips(Tab3);
			sTab += @"\tx" + InchesToTwips(Tab4);

			//output result
			return sTab + " ";
		}
		/// <summary>
		/// Insert a single tab character into the document
		/// </summary>
		/// <returns>String containing a single RTF tab tag.</returns>
		public string Tab()
		{
			return @"\tab ";
		}
		/// <summary>
		/// Insert multiple tabs into the document
		/// </summary>
		/// <param name="NumberOfTabs">Number of tab characters to 
		/// insert (max of 10)</param>
		/// <returns>String containing RTF tab tags (max of 10).</returns>
		public string Tab(int NumberOfTabs)
		{
			//initialize variables
			int iMax = 1;
			string sTab = "";

			//do not allow more than 10 tabs in a row
			if (NumberOfTabs > 10)
			{
				iMax = 10;
			}
			else if(NumberOfTabs == 0)
			{
				iMax = 1;
			}
			else
			{
				iMax = NumberOfTabs;
			}

			//loop to concat all strings together
			for (int iCount = 0; iCount < iMax; iCount ++)
			{
				sTab += @"\tab"; 
			}

			//output string
			return sTab + " ";

		}
		
		/// <summary>
		/// Creates required starting information for the RTF document
		/// </summary>
		/// <param name="DefaultFont">RtfWriter.eFontType value</param>
		/// <returns>String containing information and tags to begin RTF document</returns>
		public string StartDocument(RtfWriter.eFontType DefaultFont)
		{
			//declare variables
			string sFont;

			//convert DefaultFont to string for use in function
			sFont = Convert.ToString((int)DefaultFont);

			//output string
			return fnDocStart(sFont) + " ";
		}
		/// <summary>
		/// Creates required starting information for the RTF document and 
		/// allows user to set page margins also.
		/// </summary>
		/// <param name="DefaultFont">RtfWriter.eFontType value</param>
		/// <param name="LeftMargin">The size (in inches) of the left margin</param>
		/// <param name="RightMargin">The size (in inches) of the right margin</param>
		/// <param name="TopMargin">The size (in inches) of the top margin</param>
		/// <param name="BottomMargin">The size (in inches) of the bottom margin</param>
		/// <returns>String containing information and tags to begin RTF document</returns>
		public string StartDocument(RtfWriter.eFontType DefaultFont, double LeftMargin, 
			double RightMargin, double TopMargin, double BottomMargin)
		{
			//declare variables
			string sStart;
			string sFont;

			//convert DefaultFont to string for use in function
			sFont = Convert.ToString((int)DefaultFont);

			//call function to get beginning document info(font table, color table, etc)
			sStart = fnDocStart(sFont);

			//call function to add page margins
			sStart += fnPageMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);

			//output string
			return sStart + " ";
		}
		/// <summary>
		/// Start a bulleted list.  Bullet color can be chosen from color list.
		/// Bullet size can be between 6 pts and 20 pts.  Last list item must
		/// be followed by the SetBulletOff method.
		/// </summary>
		/// <param name="BulletColor">Chosen from the eColor enumerator</param>
		/// <param name="BulletSize">Between 6 pts and 20 pts</param>
		/// <param name="BulletIndent">Distance (in inches) between left margin and bullet</param>
		/// <param name="BulletTextSeperation">Distince (in inches) between bullet and start of text</param>
		/// <returns>String containing RTF bullet tags</returns>
		public string SetBulletOn(RtfWriter.eColor BulletColor, int BulletSize, 
			double BulletIndent, double BulletTextSeperation)
		{
			//declare variables
			string sBltClr;
			string sBltSz;
			string sBltTxt;
			string sIndent;
			int iBts;
			int iBi;
			int iIndent;
			int iFirstLine;

			//get bullet color
			sBltClr = @"\pncf" + ((int)BulletColor).ToString();

			//get bullet size
			if ((int)BulletSize < 6)
			{
				sBltSz = @"\pnfs12";
			}
			else if ((int)BulletSize > 20)
			{
				sBltSz = @"\pnfs40";
			}
			else
			{
				sBltSz = @"\pnfs" + ((int)BulletSize * 2).ToString();
			}

			//get calculate Bullet indent, and seperation between bullet/text 
			iBi = Convert.ToInt32(InchesToTwips(BulletIndent));
			iBts = Convert.ToInt32(InchesToTwips(BulletTextSeperation));
			            
			iIndent = iBi + iBts;
			iFirstLine = -(iBts);

			//save indent info
			sIndent = @"\fi" + iFirstLine.ToString() + @"\li" + iIndent.ToString();

			//create output string
			sBltTxt = "{\\*\\pn\\pnlvlblt\\pnf2\\pnindent0" + sBltClr + sBltSz +
				"{\\pntxtb\\'B7}}" + sIndent;

			//			return result
			return sBltTxt + " ";
		}
		/// <summary>
		/// Start a bulleted list.  Bullets default to same color and 
		/// font size as text.  Last list item must be followed by the SetBulletOff method.
		/// </summary>
		/// <param name="BulletIndent">Distance (in inches) between left margin and bullet</param>
		/// <param name="BulletTextSeperation">Distince (in inches) between bullet and start of text</param>
		/// <returns>String containing RTF bullet tags</returns>
		public string SetBulletOn(double BulletIndent, double BulletTextSeperation)
		{
			//declare variables
			string sBltTxt;
			int iBi;
			int iBts;
			int iIndent;
			int iFirstLine;

			//create basic bullet text
			sBltTxt = @"{\*\pn\pnlvlblt\pnf2\pnindent0{\pntxtb\'B7}}";

			//calculate Bullet indent, and seperation between bullet/text
			iBi = Convert.ToInt32(InchesToTwips(BulletIndent));
			iBts = Convert.ToInt32(InchesToTwips(BulletTextSeperation));
           
			iIndent = iBi + iBts;
			iFirstLine = -(iBts);

			//add indent info to bullet text
			sBltTxt += @"\fi" + iFirstLine.ToString() + @"\li" + iIndent.ToString();

			//output results
			return sBltTxt;	

		}
		/// <summary>
		/// Ends a bulleted list.
		/// </summary>
		/// <returns>String containing RTF tag to end a bulleted list</returns>
		public string SetBulletOff()
		{
			return @"\par\pard ";
		}
		/// <summary>
		/// Start a list.  Bullets default to  font size as text.  
		/// Last list item must be followed by the SetBulletOff method.
		/// </summary>
		/// <param name="ListIndent">Distance (in inches) between left margin and list character</param>
		/// <param name="ListTextSeperation">Distince (in inches) between list character and start of text</param>
		/// <param name="ListCharacterFont">The font of the list character</param>
		/// <param name="ListType">An eListType type</param>
		/// <param name="ListCharacterSuffix">An eListCharacterSuffix type</param>
		/// <returns>String containing RTF list tags</returns>
		public string SetListOn(double ListIndent, double ListTextSeperation, 
				eFontType ListCharacterFont, eListType ListType, eListCharacterSuffix ListCharacterSuffix)
		{
			//declare variables
			string sListText;
			string sListCharSuffix;
			string sListType;
			int iCharIndent;
			int iTextSeperation;
			int iIndent;
			int iFirstLine;

			//set the list type and start character
			switch (ListType)
			{
				case eListType.LowercaseAlpha:
				{
					sListType = @"\pnlcltr\pnstart1";
					break;
				}
				case eListType.UppercaseAlpha:
				{
					sListType = @"\pnucltr\pnstart1";
					break;
				}
				default:
				{
					sListType = @"\pndec\pnstart1";
					break;
				}
			}

			//set the list char suffix
			switch (ListCharacterSuffix)
			{
				case eListCharacterSuffix.None:
				{
					sListCharSuffix = "";
					break;
				}
				case eListCharacterSuffix.Parentheses:
				{
					sListCharSuffix = ")";
					break;
				}
				default:
				{
					sListCharSuffix = ".";
					break;
				}
			}

			//create basic list text
			sListText = @"{\*\pn\pnlvlbody\pnf" + (int)ListCharacterFont + @"\pnindent0{" +
				sListType + @"\pntxta " + sListCharSuffix + @"}}";

			//calculate list indent, and seperation between list character & text
			iCharIndent = Convert.ToInt32(InchesToTwips(ListIndent));
			iTextSeperation = Convert.ToInt32(InchesToTwips(ListTextSeperation));
           
			iIndent = iCharIndent + iTextSeperation;
			iFirstLine = -(iTextSeperation);

			//add indent info to bullet text
			sListText += @"\fi" + iFirstLine.ToString() + @"\li" + iIndent.ToString();

			//output results
			return sListText;	

		}
		/// <summary>
		/// Ends a list.
		/// </summary>
		/// <returns>String containing RTF tag to end a list</returns>
		public string SetListOff()
		{
			return @"\par\pard ";
		}
		/// <summary>
		/// Ends the RTF Document.  Failure to end the document with this tag
		/// will result in a corrupt RTF document 
		/// </summary>
		/// <returns>String containing the tag to end the RTF document</returns>
		public string EndDocument()
		{
			return "}";
		}
		/// <summary>
		/// Removes invalid RTF characters and replaces with the valid RTF character
		/// string
		/// </summary>
		/// <param name="RtfString">StringBuilder object containing the string to be analyzed</param>
		/// <returns>StringBuilder object containing the valid string</returns>
		public StringBuilder ReplaceInvalidRtfCharacters(StringBuilder RtfString)
		{
			char[] myChar = RtfString.ToString().ToCharArray();
			StringBuilder sbNew = new StringBuilder(myChar.Length);

			//replace char > 128 (non-standard ASCII) with RTF safe codes
			foreach(char c in myChar)
			{
				if(c < 128) //standard ASCII
				{
					switch(c)
					{
						case (char)10: //LineFeed symbol
						{
							break;
						}
						case (char)13: //CarriageReturn symbol
						{
							sbNew.Append(@"\par ");
							break;
						}
						default: //regular RTF ASCII
						{
							sbNew.Append(c);
							break;
						}
					}
				}
				else if(c >= 128 && c < 256) //rtf hex code (8-bit unicode)
				{
					sbNew.Append(@"\'" + Convert.ToString(c, 16));
				}
				else //unicode, but for older readers, "¿" will show since they don't understand 16-bit unicode (¿ = "\'BF" in 8-bit unicode)
				{
					sbNew.Append(@"\u" + Convert.ToString(c, 10) + @"\'BF");
				}
			}

			//output clean RTF
			return sbNew;
		}
		/// <summary>
		/// Removes invalid RTF characters and replaces with the valid RTF character
		/// string
		/// </summary>
		/// <param name="RtfString">String containing the text to be analyzed</param>
		/// <returns>String containing the valid text</returns>
		public string ReplaceInvalidRtfCharacters(string RtfString)
		{
			//declare objects
			StringBuilder sb = new StringBuilder(RtfString);

			//call overloaded method to perform functions
			sb = ReplaceInvalidRtfCharacters(sb);
			return sb.ToString();
		}	
		/// <summary>
		/// Inserts a JPG, GIF or BMP image into the RTF document at a specified 
		/// location on the current page.  GIF and BMP images are converted to JPG format.  
		/// If images are larger than the specified height/width, then they will be
		/// resized (keeping their original aspect ratios) to fit the specified area.
		/// </summary>
		/// <param name="ImgByteArray">Binary image data in byte array.  The original image
		/// must be have been in one of the following formats: JPG, GIF, or BMP</param>
		/// <param name="LeftEdgeLoc">The distance (in inches) from the left margin 
		/// that the left side of the image should be located.
		/// </param>
		/// <param name="TopEdgeLoc">
		///	The distance (in inches) from the Top margin 
		///	that the top side of the image should be located.</param>
		/// <param name="MaxHeight">
		///	Maximum height (in inches) the image should be sized on the
		///	document.
		/// </param>
		/// <param name="MaxWidth">Maximum width (in inches) that image should be sized on the
		/// document.</param>
		/// <returns>String containing the the binary image information converted
		/// to a RTF JPG hex string format.</returns>
		public string InsertJpg(byte[] ImgByteArray, double LeftEdgeLoc, double TopEdgeLoc, 
			double MaxHeight, double MaxWidth)
		{
			//declare variables
			string sImgHexString = "";
			string sRtfImgHeader = "";
			string sRtfImgFooter = "";
			string sScaledImgHeight = "";
			string sScaledImgWidth = "";
			string sImgLeft = "";
			string sImgRight = "";
			string sImgTop = "";
			string sImgBottom = "";
			int ArraySize = ImgByteArray.Length;
			int iHtDpi = 0;
			int iWtDpi = 0;
			
			//load filestream
			MemoryStream imageStream = new MemoryStream(ImgByteArray, 0, ArraySize);

			//create image object
			System.Drawing.Image img = System.Drawing.Image.FromStream(imageStream);

			/*----- if BMP or GIF, convert image to JPG ------*/
			if(img.RawFormat.Guid == Imaging.ImageFormat.Gif.Guid || 
				img.RawFormat.Guid == Imaging.ImageFormat.Bmp.Guid)
			{
				//create Encoder parameters, set compression quality to 100 (highest quality)
				//since I don't know the quality of the image to begin with, I don't want to
				//make it worse.
				Imaging.Encoder imgEnc = Imaging.Encoder.Quality;
				Imaging.EncoderParameter ratio = new Imaging.EncoderParameter(imgEnc, 100L);
			
				//add the quality parameter
				Imaging.EncoderParameters codecParams = new Imaging.EncoderParameters(1);
				codecParams.Param[0] = ratio;

				//convert the image to jpeg
				MemoryStream newImageStream = new MemoryStream();
				img.Save(newImageStream, GetEncoderInfo("image/jpeg"), codecParams);
				//img.Save(newImageStream, ImageFormat.Jpeg);

				//reload image
				img.Dispose();
				img = System.Drawing.Image.FromStream(newImageStream);

				//load up the byte array with the new info
				ImgByteArray = new byte[newImageStream.Length];
				int i = 0;
				newImageStream.Position = 0;
				i = newImageStream.Read(ImgByteArray, 0, (int)newImageStream.Length);
			}
			/*-------------------------------------------------*/
			
			//check image format, continue if jpeg
			if (img.RawFormat.Guid == Imaging.ImageFormat.Jpeg.Guid)
			{
				//get image resolution
				iHtDpi = (int)img.HorizontalResolution;
				iWtDpi = (int)img.VerticalResolution;

				//if resolutions are equal and > 300 dpi, than default to 300 dpi
				//for both
				if ((iHtDpi == iWtDpi) && (iHtDpi > 300))
				{
					iHtDpi = 300;
					iWtDpi = 300;
				}

				//convert MaxWidth/MaxHeight to Size object
				Size MaxW_MaxH = new Size(Convert.ToInt32(MaxWidth * iWtDpi),
					Convert.ToInt32(MaxHeight * iHtDpi));

				//Scale the image
				Size ScaledImage = fnScaleImage(img.Size, MaxW_MaxH);

				//convert scaled image specs from pixels to twips
				sScaledImgHeight = InchesToTwips((double)ScaledImage.Height / (double)iHtDpi); 
				sScaledImgWidth = InchesToTwips((double)ScaledImage.Width / (double)iWtDpi); 

				//convert Byte array to hex string
				sImgHexString = BinaryToHex(ImgByteArray);

				//set image borders
				sImgLeft = InchesToTwips(LeftEdgeLoc);
				sImgRight = Convert.ToString(Convert.ToDouble(sImgLeft) + Convert.ToDouble(sScaledImgWidth));
				sImgTop = InchesToTwips(TopEdgeLoc);
				sImgBottom = Convert.ToString(Convert.ToDouble(sImgTop) + Convert.ToDouble(sScaledImgHeight));

				//create ImageHeader
				sRtfImgHeader = @"{\shp{\*\shpinst\shpleft" + sImgLeft + @"\shptop" + sImgTop + 
					@"\shpright" + sImgRight + @"\shpbottom" + sImgBottom + 
					@"\shpwr3 {\sp{\sn shapeType}{\sv 75}}{\sp{\sn lineColor}" +
					@"{\sv 1}}{\sp{\sn fLine}{\sv 1}}{\sp{\sn lineWidth}{\sv 6350}}" +
					@"{\sp{\sn pib}{\sv {\pict\jpegblip ";
				sRtfImgFooter = @"}}}}}";
				
				//output string
				return sRtfImgHeader + sImgHexString + sRtfImgFooter;
			}
			//image isn't a JPG, return empty string
			else
			{
				return "";
			}
			
		}
		/// <summary>
		/// Convert an image to a hex string and inserts it into the document header
		/// </summary>
		/// <param name="ImagePath">Physical path to the image</param>
		/// <param name="TopHeaderMargin">Distance (in inches) between top of header and 
		/// the top edge of the document.  Must be between 0.1 and 1.0 inches</param>
		/// <returns>RTF string containing RTF header information</returns>
		public string InsertDocHeader(string ImagePath, double TopHeaderMargin)
		{
			//declare variables
			string sHeaderMargin;
			
			//format top header margin
			if(TopHeaderMargin < 0.1D)
			{
				sHeaderMargin = @"\headery" + InchesToTwips(0.1D) + " ";
			}
			else if(TopHeaderMargin > 1D)
			{
				sHeaderMargin = @"\headery" + InchesToTwips(1D) + " ";
			}
			else
			{
				sHeaderMargin = @"\headery" + InchesToTwips(TopHeaderMargin) + " ";
			}
			
			//create BinaryReader/FileStream objects from image
			FileStream imageStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(imageStream);

			//load imageStream to byte array
			byte[] imageData = br.ReadBytes((int)imageStream.Length);

			//close FileStream/BinaryReader
			imageStream.Close();
			br.Close();

			//create header (use rtf tags and convert byte array to hex string)
			return sHeaderMargin + @"{\header {\*\shppict{\pict\jpegblip " + 
					BinaryToHex(imageData) + "}}}";
		}
		/// <summary>
		/// Convert an image to a hex string and inserts it into the document footer
		/// </summary>
		/// <param name="ImagePath">Physical path to the image</param>
		/// <param name="BottomFooterMargin">Distance (in inches) between bottom of footer and 
		/// the bottom edge of the document.  Must be between 0.1 and 1.0 inches</param>
		/// <param name="FooterText">Footer text (can be null or empty string)</param>
		/// <returns>RTF string containing RTF footer information</returns>
		public string InsertDocFooter(string ImagePath, double BottomFooterMargin, string FooterText)
		{
			//declare variables
			string sFooterMargin;

			//if the footer is a null value, switch to an empty string
			if(FooterText == null)  
			{
				FooterText = "";
			}
			
			//format top header margin
			if(BottomFooterMargin < 0.1D)
			{
				sFooterMargin = @"\footery" + InchesToTwips(0.1D) + " ";
			}
			else if(BottomFooterMargin > 1D)
			{
				sFooterMargin = @"\footery" + InchesToTwips(1D) + " ";
			}
			else
			{
				sFooterMargin = @"\footery" + InchesToTwips(BottomFooterMargin) + " ";
			}
			
			//create BinaryReader/FileStream objects from image
			FileStream imageStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(imageStream);

			//load imageStream to byte array
			byte[] imageData = br.ReadBytes((int)imageStream.Length);

			//close FileStream/BinaryReader
			imageStream.Close();
			br.Close();

			//create footer (use rtf tags and convert byte array to hex string)
			return sFooterMargin + @"{\footer {\*\shppict{\pict\jpegblip " + 
				BinaryToHex(imageData) + "}}" + FooterText + "}";
		}
		/// <summary>
		/// Inserts a textbox into the document at a specified location.
		/// </summary>
		/// <param name="Text">Text to be inserted into the textbox</param>
		/// <param name="LeftEdgeLoc">The distance (in inches) from the left margin 
		/// that the left side of the Textbox should be located.
		/// </param>
		/// <param name="TopEdgeLoc">
		///	The distance (in inches) from the Top margin 
		///	that the top side of the Textbox should be located.</param>
		/// <param name="Height">
		///	Height (in inches) the Textbox should be sized on the
		///	document
		/// </param>
		/// <param name="Width">Width (in inches) that Textbox should be sized on the
		/// document</param>
		/// <returns>String containing the RTF textbox information at a specified
		/// location a page</returns>
		public string InsertTextbox(string Text, double LeftEdgeLoc, double TopEdgeLoc, 
			double Height, double Width)
		{
			//Declare variables
			StringBuilder sbTb = new StringBuilder();

			//set image borders
			string sTbLeft = InchesToTwips(LeftEdgeLoc);
			string sTbRight = Convert.ToString(Convert.ToDouble(sTbLeft) + Convert.ToDouble(InchesToTwips(Width)));
			string sTbTop = InchesToTwips(TopEdgeLoc);
			string sTbBottom = Convert.ToString(Convert.ToDouble(sTbTop) + Convert.ToDouble(InchesToTwips(Height)));

			
			//Create Textbox header
			sbTb.Append(@"{\shp{\*\shpinst\shpleft" + sTbLeft + @"\shptop" + sTbTop +
				@"\shpright" + sTbRight + @"\shpbottom" + sTbBottom + 
				@"\shpfhdr0\shpwr3\shpwrk0\shpfblwtxt0\shpz0{\sp{\sn shapeType}{\sv 202}}" +
				@"{\shptxt ");

			//add text and footer
			sbTb.Append(Text + @"}}}");

			//output results
			return sbTb.ToString();
	
		}
		
		/// <summary>
		/// Outputs data inside an RTF table
		/// </summary>
		/// <param name="dt">Datatable containing the info to format</param>
		/// <param name="ColumnWidths">Array containing the column widths (in inches) for
		/// each column in the DataTable.</param>
		/// <param name="IncludeBorders">Boolean indicating if table has borders</param>
		/// <returns>String containing the RTF markup and data</returns>
		public string CreateTable(DataTable dt, double[] ColumnWidths, bool IncludeBorders)
		{
			//Variables
			double dTemp = 0;
			string sColumnWidthSettings;
			string sRowBorders;
			string sCellBorders;
			StringBuilder sbTemp = new StringBuilder();
			StringBuilder sbTable = new StringBuilder();
			
			//create the border strings
			if(IncludeBorders == true)
			{
				sRowBorders = @"\trbrdrt\brdrs\brdrw10" +
					@"\trbrdrl\brdrs\brdrw10" +
					@"\trbrdrb\brdrs\brdrw10" +
					@"\trbrdrr\brdrs\brdrw10" +
					@"\trbrdrh\brdrs\brdrw10" +
					@"\trbrdrv\brdrs\brdrw10 ";
				sCellBorders = @"\clbrdrt\brdrs\brdrw10" +
					@"\clbrdrl\brdrs\brdrw10" +
					@"\clbrdrb\brdrs\brdrw10" +
					@"\clbrdrr\brdrs\brdrw10 ";
			}
			else //no borders, use empty strings
			{
				sRowBorders = "";
				sCellBorders = "";
			}

			//Create the column width settings tags, if null, set each to zero
			if(ColumnWidths != null)
			{
				foreach(double dWidth in ColumnWidths)
				{
					sbTemp.Append(sCellBorders);
					sbTemp.Append(@"\cellx" + InchesToTwips(dWidth + dTemp));

					//update dTemp
					dTemp = dTemp + dWidth;
				}
			}
			else  //no column widths set, will autofit
			{
				for(int i = 0; i < dt.Columns.Count; i++)
				{
					sbTemp.Append(@"\cellx" + Convert.ToString(i + 1)); //
				}
			}

			//save column tags and reset string builder
			sColumnWidthSettings = sbTemp.ToString();
			sbTemp.Remove(0, sbTemp.Length);

			//start the table
			sbTable.Append(@"{");

			//write out each row
			foreach(DataRow myRow in dt.Rows)
			{
				//row headers
				sbTable.Append(@"\trowd\trautofit1\intbl");
				sbTable.Append(sRowBorders);
				sbTable.Append(sColumnWidthSettings + "{ ");

				//row info
				foreach(DataColumn col in dt.Columns)
				{
					sbTable.Append(fnRemoveSpecialChar(myRow[col].ToString()) + @"\cell ");
				}

				//close row
				sbTable.Append(@"}{\row }");
			}

			//close the table
			sbTable.Append(@"}");
		
			//output results
			return sbTable.ToString();		

		}
		

		/// <summary>
		/// Outputs data inside an RTF table.  Columns are set to autofit.
		/// </summary>
		/// <param name="dt">Datatable containing the info to format</param>
		/// <param name="IncludeBorders">Boolean indicating if table has borders</param>
		/// <returns>String containing the RTF markup and data</returns>
		public string CreateTable(DataTable dt, bool IncludeBorders)
		{
			return CreateTable(dt, null, IncludeBorders);
		}
		
	
		/// <summary>
		/// Outputs data inside an RTF table
		/// </summary>
		/// <param name="DataRows">DataRow array containing the rows to format</param>
		/// <param name="ColumnWidths">Array containing the column widths (in inches) for
		/// each column in the DataTable.  (null allowed: autofit columns)</param>
		/// <param name="ColumnNames">Array containing the column names that will be included
		/// in the RTF Table.  Only columns that are listed in the array extracted from the
		/// DataRow and used. (null allowed: defaults to all columns)</param> 
		/// <param name="IncludeBorders">Boolean indicating if table has borders</param>
		/// <returns>String containing the RTF markup and data</returns>
		public string CreateTable(DataRow[] DataRows, double[] ColumnWidths, string[] ColumnNames, bool IncludeBorders)
		{
			//Variables
			double dTemp = 0;
			string sColumnWidthSettings;
			string sRowBorders;
			string sCellBorders;
			StringBuilder sbTemp = new StringBuilder();
			StringBuilder sbTable = new StringBuilder();
			
			//create the border strings
			if(IncludeBorders == true)
			{
				sRowBorders = @"\trbrdrt\brdrs\brdrw10" +
					@"\trbrdrl\brdrs\brdrw10" +
					@"\trbrdrb\brdrs\brdrw10" +
					@"\trbrdrr\brdrs\brdrw10" +
					@"\trbrdrh\brdrs\brdrw10" +
					@"\trbrdrv\brdrs\brdrw10 ";
				sCellBorders = @"\clbrdrt\brdrs\brdrw10" +
					@"\clbrdrl\brdrs\brdrw10" +
					@"\clbrdrb\brdrs\brdrw10" +
					@"\clbrdrr\brdrs\brdrw10 ";
			}
			else //no borders, use empty strings
			{
				sRowBorders = "";
				sCellBorders = "";
			}

			//Create the column width settings tags, if null, set each to zero
			if(ColumnWidths != null)
			{
				foreach(double dWidth in ColumnWidths)
				{
					sbTemp.Append(sCellBorders);
					sbTemp.Append(@"\cellx" + InchesToTwips(dWidth + dTemp));

					//update dTemp
					dTemp = dTemp + dWidth;
				}
			}
			else  //no column widths set, will autofit
			{
				if(ColumnNames == null) //all columns
				{
					for(int i = 0; i < DataRows[0].Table.Columns.Count; i++) 
					{
						sbTemp.Append(@"\cellx" + Convert.ToString(i + 1));
					}
				}
				else //only passed in columns
				{
					for(int i = 0; i < ColumnNames.Length; i++) 
					{
						sbTemp.Append(@"\cellx" + Convert.ToString(i + 1));
					}
				}
			}

			//save column tags and reset string builder
			sColumnWidthSettings = sbTemp.ToString();
			sbTemp.Remove(0, sbTemp.Length);

			//start the table
			sbTable.Append(@"{");

			//write out each row
			foreach(DataRow dr in DataRows)
			{
				//row headers
				sbTable.Append(@"\trowd\trautofit1\intbl");
				sbTable.Append(sRowBorders);
				sbTable.Append(sColumnWidthSettings + "{ ");

				//row info (columns)
				if(ColumnNames == null)  //all columns
				{
					foreach(DataColumn col in dr.Table.Columns)
					{
						sbTable.Append(fnRemoveSpecialChar(dr[col].ToString()) + @"\cell ");
					}
				}
				else //only passed in column names
				{
					foreach(string sColName in ColumnNames)
					{
						sbTable.Append(fnRemoveSpecialChar(dr[sColName].ToString()) + @"\cell ");
					}

				}

				//close row
				sbTable.Append(@"}{\row }");
			}

			//close the table
			sbTable.Append(@"}");
		
			//output results
			return sbTable.ToString();		

		}

		#endregion

		public string Italicize(string text)
		{
			return string.Format("{0}{1}{2}",
				SetFontStyle(eFontStyle.Italic, eSetting.On),
				ReplaceInvalidRtfCharacters(text),
				SetFontStyle(eFontStyle.Italic, eSetting.Off)
			);
		}

		public string Embolden(string text)
		{
			return string.Format("{0}{1}{2}",
				SetFontStyle(eFontStyle.Bold, eSetting.On),
				ReplaceInvalidRtfCharacters(text),
				SetFontStyle(eFontStyle.Bold, eSetting.Off)
			);
		}


	}
}


