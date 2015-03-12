using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Strata {
    public class Repository : Strata.Extension{
        //public Model Create(Dictionary<string, dynamic> obj) {
        //    var model = this.Create();
        //    model.Bind(obj);
        //    return model;
        //}

        //public virtual Model Create() {
        //    var model = (Model)Activator.CreateInstance(this.ModelClass);
        //    return model;
        //}

        //public Type ModelClass {
        //    get { return typeof(Member); }
        //}

        //public List<int> Keychain() {
        //    var sql = "SELECT ID FROM accounts;";
        //    var keys = this.Scalars<int>(sql);
        //    return keys;// as List<int>;
        //}

        public List<Model> Get() {
            var query = this.SelectQuery();
            var models = query.Records<Model>();
            return models;
        }

        public List<Model> Get(List<int> keys) {
            var query = this.SelectQuery();
            query.Where("id", keys);
            var models = query.Records<Model>();
            return models;
        }

        public Model Save(Model model) {
            if (model.id == -1){
                var id = this.Insert(model);
                if (model.id == -1)
                    model.id = id;
            } else { 
                this.Update(model);
            }
            return model;
        }

        public virtual int Insert(Model model) {
            var id = this.Query("INSERT INTO accounts (label) VALUES (@label);", model).Insert();
            return id;
        }

        public virtual void Update(Model model) {
            this.Query("UPDATE accounts SET label=@label WHERE id=@id;", model).Update();
        }

        public virtual Strata.DB.Query SelectQuery() {
            throw new NotImplementedException();
        }
    }
}
