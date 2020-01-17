using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BasicWPFUIPlayground
{
    /// <summary>
    /// Interaction logic for DataGridBinder.xaml
    /// </summary>
    public partial class DataGridBinder : Window
    {
        public ObservableCollection<User> users { get; set; }
        public ObservableCollection<Gender> genders { get; set; }
        ObservableCollection<KeyValuePair<int, string>> genderCol { get; set; }

        public DataGridBinder()
        {
            InitializeComponent();
            
            genders = new ObservableCollection<Gender>() {
                new Gender(){DisplayName="Male", CodedValue="M"},
                new Gender(){DisplayName="Female", CodedValue="F"},
            };

            genderCol = new ObservableCollection<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(1,"Male"),
                new KeyValuePair<int, string>(2, "Female")
            };

            users = new ObservableCollection<User>();
            users.Add(new User() { Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23), GenderList = genders, SelectedGender = genders[0], GenKeyPair = genderCol, SelGenKV = genderCol[0] });
            users.Add(new User() { Id = 2, Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17), GenderList = genders, SelectedGender = genders[1], GenKeyPair = genderCol, SelGenKV = genderCol[1] });
            users.Add(new User() { Id = 3, Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2), GenderList = genders, SelectedGender = genders[0], GenKeyPair = genderCol, SelGenKV = genderCol[0] });

            dgUsers.ItemsSource = users;
        }

        public class User
        {
            public int Id { get; set; }
            public int Size { get; set; }

            public string Name { get; set; }

            public DateTime Birthday { get; set; }

            public Gender SelectedGender { get; set; }

            public KeyValuePair<int, string> SelGenKV { get; set; }
            public ObservableCollection<KeyValuePair<int, string>> GenKeyPair { get;set;}
           

            public ObservableCollection<Gender> GenderList { get;set;}
        }

        public enum GenderEnum { Male = 0, Female = 3 };

        public class Gender
        {
            public string DisplayName { get; set; }
            public string CodedValue { get; set; }
        }

        private void DebugBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach(var user in this.users)
            {
                Console.WriteLine($"{user.Name}, {user.SelectedGender.DisplayName} , {user.SelectedGender.CodedValue}");
            }
            Console.WriteLine("It is finished");
        }
    }
}
