using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Strata {
    public class Repository : Strata.Extension{
        public Model Create(Dictionary<string, dynamic> obj) {
            var model = (Model)Activator.CreateInstance(this.ModelClass);
            model.Bind(obj);
            return model;
        }

        public Model Create() {
            var model = (Model)Activator.CreateInstance(this.ModelClass);
            return model;
        }

        public Type ModelClass {
            get { return typeof(Member); }
        }


        public List<int> Keychain() {
            var sql = "SELECT ID FROM accounts;";
            var keys = this.Scalars<int>(sql);
            return keys;// as List<int>;
        }

        public List<Model> Fetch(List<int> keys) {
            //List<string> names = keys.ConvertAll(new Converter<int, string>(delegate(int x) { return x.ToString(); }));
            var records = this.Query("SELECT * FROM accounts;").Where("id", keys).Select();
            var models = new List<Model>();

            Func<Dictionary<string, dynamic>, Model> factory = Strata.Util.Toolkit.Curry(
                (Func<Type, Dictionary<string, dynamic>, Model>)Model.Construct, this.ModelClass
            );

            foreach (var record in records) {
                var model = factory(record);
                models.Add(model);
            }
            return models;
        }

        public void Save(Model model) {
            if (model.id == -1)
                this.Insert(model);
            else
                this.Update(model);
        }
        public int Insert(Model model) {
            var id = this.Query("INSERT INTO accounts (label) VALUES (@label);", model).Insert();
            return id;
        }

        public void Update(Model model) {
            this.Query("UPDATE accounts SET label=@label WHERE id=@id;", model).Update();
        }

        public string SelectQuery {
            get {
                return "SELECT * FROM accounts;";
            }
        }
    }
}
