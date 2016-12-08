/****************************** Module Header ******************************\
Module Name:  DependsOnAttribute.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;

namespace NControl.Mvvm
{
    /// <summary>
    /// Depends on attribute.
    /// </summary>
    public class DependsOnAttribute: Attribute
    {
        /// <summary>
        /// Gets or sets the source property.
        /// </summary>
        /// <value>The source property.</value>
		public IEnumerable<String> SourceProperties {get;set;}

        /// <summary>
        /// If the property is set but the value is the same, this flag will make sure that the 
        /// notification event is called.
        /// </summary>
        /// <value><c>true</c> if raise property change for equal values; otherwise, <c>false</c>.</value>
        public bool RaisePropertyChangeForEqualValues { get; set;}

        /// <summary>
        /// Initializes a new instance of the DependsOnAttribute class.
        /// </summary>
        public DependsOnAttribute(params string[] propertyNames)
        {
			SourceProperties = propertyNames;
        }

        /// <summary>
        /// Initializes a new instance of the DependsOnAttribute class.
        /// </summary>
        public DependsOnAttribute(bool raisePropertyChangeForEqualValues, params string[] propertyNames)
        {
			SourceProperties = propertyNames;
            RaisePropertyChangeForEqualValues = raisePropertyChangeForEqualValues;
        }
    }
}

