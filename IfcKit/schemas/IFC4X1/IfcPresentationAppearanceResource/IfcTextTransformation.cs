// This file was automatically generated from IFCDOC at www.buildingsmart-tech.org.
// IFC content is copyright (C) 1996-2018 BuildingSMART International Ltd.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace BuildingSmart.IFC.IfcPresentationAppearanceResource
{
	[Guid("fa000009-bbfe-4524-a950-af6866dcecb9")]
	public partial struct IfcTextTransformation
	{
		[XmlText]
		public String Value;
	
		public IfcTextTransformation(String value)
		{
			this.Value = value;
		}
	}
	
}