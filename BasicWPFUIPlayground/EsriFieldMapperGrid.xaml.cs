using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WoodsideTool.Transaction.Model;

namespace BasicWPFUIPlayground
{
    /// <summary>
    /// Interaction logic for EsriFieldMapperGrid.xaml
    /// </summary>
    public partial class EsriFieldMapperGrid : Window
    {
        private ObservableCollection<EsriFieldMapper> _fieldMapper;
        public ObservableCollection<EsriFieldMapper> FieldMapper
        {

            get => _fieldMapper;
            set
            {
                _fieldMapper = value;
                
            }
        }

        public EsriFieldMapperGrid()
        {
            InitializeComponent();
     string fieldMapperstr = File.ReadAllText(@"D:\temp\fieldMapper.json");
            this.FieldMapper = JsonConvert.DeserializeObject<ObservableCollection<EsriFieldMapper>>(fieldMapperstr);
            this.dgFieldMapper.ItemsSource = this.FieldMapper;
        }
    }
}
