using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WoodsideTool.Transaction.Model
{
    public class EsriFieldMapper : NotifyPropertyBase
    {
        private string _importFieldName;
        private bool _isExport;
        private int? _size;
        private KeyValuePair<int, string> _selFieldType;
        private int _fieldIndex;
        private string _desireFieldName;
        private string _alias;
        private string _defaultValue;
        private string _additionalValue;
        private string _sample;
        private ObservableCollection<KeyValuePair<int, string>> _availableFieldType;

        public string ImportFieldName
        {
            get => _importFieldName;
            set
            {
                _importFieldName = value;
                OnPropertyChanged();
            }
        }

        public string Sample
        {
            get => _sample;
            set
            {
                _sample = value;
                OnPropertyChanged();
            }
        }
        public bool IsExport
        {
            get => _isExport;
            set
            {
                _isExport = value;
                OnPropertyChanged();
            }
        }
        public int? Size
        {
            get => _size;
            set
            {
                _size = value; OnPropertyChanged();
            }
        }

        public KeyValuePair<int, string> SelectedFieldType
        {
            get => _selFieldType;
            set
            {
                _selFieldType = value;
                OnPropertyChanged();
            }
        }
        public int FieldIndex
        {
            get => _fieldIndex;
            set
            {
                _fieldIndex = value;
                OnPropertyChanged();
            }
        }
        public string DesireFieldName
        {
            get => _desireFieldName;
            set
            {
                _desireFieldName = value;
                OnPropertyChanged();
            }
        }
        public string DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                OnPropertyChanged();
            }
        }
        public string AliasFieldName
        {
            get => _alias;
            set
            {
                _alias = value;
                OnPropertyChanged();
            }
        }

        public string AdditionalValue
        {
            get=>_additionalValue;
            set
            {
                _additionalValue = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<int, string>>  AvailableFieldType
        {
            get => _availableFieldType;
            set
            {
                _availableFieldType = value;
                OnPropertyChanged();
            }
        }

        public EsriFieldMapper()
        {
            this.AvailableFieldType = new ObservableCollection<KeyValuePair<int, string>>();
        }


        public string ToArcPyFieldInfoValueTable()
        {
            string fieldSize = "''";
            if (this.Size.HasValue)
            {
                fieldSize = $"{this.Size}";
            }

            if(this.SelectedFieldType.Value.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!this.Size.HasValue)
                {
                    fieldSize = "255";
                }
            }
            else
            {
                //Other field type does not require field size
                fieldSize = "''";
            }
            return $"'{this.DesireFieldName}' '{this.SelectedFieldType.Value}' '{this.DesireFieldName}' {fieldSize} '{this.DefaultValue}' '{this.AdditionalValue}'";
        }
    }
}
