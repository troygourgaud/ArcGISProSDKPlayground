using ArcGIS.Core;
using ArcGIS.Core.Data;
using ArcGIS.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstAddOn_ArcgisPro.DTO
{
    public class CustomField 
    {
        private FieldInfo _fieldInfo;

        /// <summary>
        /// Gets the alias name of the field.
        /// </summary>
        public string AliasName
        {
            get
            {
                return this._fieldInfo.aliasName;
            }
           
        }

        /// <summary>
        /// Gets the <see cref="T:ArcGIS.Core.Data.FieldType" /> of the field.
        /// </summary>
        public FieldType FieldType
        {
            get
            {
                return (FieldType)this._fieldInfo.fieldType;
            }
            
        }

        /// <summary>
        /// Gets a value indicating whether this field has a default value.
        /// </summary>
        public bool HasDefaultValue
        {
            get
            {
                return this._fieldInfo.hasDefaultValue;
            }
            set
            {
                this._fieldInfo.hasDefaultValue = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this field's domain is fixed.
        /// </summary>
        public bool IsDomainFixed
        {
            get
            {
                return this._fieldInfo.isDomainFixed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this field should be treated as read only by supported clients.
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return this._fieldInfo.isEditable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this field can contain null values.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return this._fieldInfo.isNullable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a required field.
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return this._fieldInfo.isRequired;
            }
        }

        /// <summary>
        /// Gets a value indicating the maximum length in bytes for values described by the field.
        /// </summary>
        public int Length
        {
            get
            {
                return this._fieldInfo.length;
            }
        }

        /// <summary>
        /// Gets the model name of the field.
        /// </summary>
        public string ModelName
        {
            get
            {
                return this._fieldInfo.modelName;
            }
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string Name
        {
            get
            {
                return this._fieldInfo.name;
            }
        }

        /// <summary>
        /// Gets a value indicating the precision for field values.
        /// </summary>
        /// <remarks>
        /// Precision is the number of digits in a number. For example, the number 56.78 has a precision of 4. Precision is only valid for fields that are numeric. 
        /// Precision is always returned as 0 from File geodatabase fields.
        /// </remarks>
        public int Precision
        {
            get
            {
                return this._fieldInfo.precision;
            }
        }

        /// <summary>
        /// Gets a value indicating the scale for field values.
        /// </summary>
        /// <remarks>
        /// Scale is the number of digits to the right of the decimal point in a number. For example, the number 56.78 has a scale of 2. 
        /// Scale applies only to fields that are double.  Scale is always returned as 0 from personal or File geodatabase fields.
        /// </remarks>
        public int Scale
        {
            get
            {
                return this._fieldInfo.scale;
            }
        }
    }
}
