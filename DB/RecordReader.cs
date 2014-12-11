using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Data;
using System.Data.Common;
namespace Strata.DB {
    public class RecordReader : DbDataReader, INullableReader {
        private delegate T Conversion<T>(int ordinal);
        #region -------- CONSTRUCTOR/VARIABLES --------
        private bool _aborted = false;
        private DbDataReader _reader;
        private Dictionary<string, int> _columns;
        public RecordReader(DbDataReader reader) {
            this._reader = reader;
            int fieldCount = reader.FieldCount;
            this._columns = new Dictionary<string, int>(fieldCount, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < fieldCount; i++) {
                string name = reader.GetName(i);
                if (this._columns.ContainsKey(name))
                    throw new DuplicateNameException("Found duplicate column name from Data Reader. Column Name: " + name);

                this._columns.Add(name, i);
                //this.m_ordinals.Add(dataReader.GetName(i), i);
            }
        }
        #endregion


        #region -------- PUBLIC - Abort --------
        /// <summary>
        /// Closes the reader and a flag that will prevent the Read() method from being executed
        /// </summary>
        public void Abort() {
            if (this._aborted)
                return;
            this._aborted = true;
            this._reader.Close();
        }
        #endregion


        public bool HasField(string name) {
            Contract.Requires(!String.IsNullOrEmpty(name), "The field name is null/empty!");
            return this._columns.ContainsKey(name);
        }
        public bool HasField(int i) {
            Contract.Requires(i > 0, "The field index must be greater than or equal to zero!");
            return (this._columns.Count <= i) ? true : false;
        }
        public string[] Fields {
            get {
                return this._columns.Keys.ToArray();
            }
        }



        public override bool GetBoolean(int i) {
            return this._reader.GetBoolean(i);
        }

        public bool GetBoolean(string name) {
            return this.GetBoolean(this.GetOrdinal(name));
        }

        public Nullable<bool> GetNullableBoolean(int index) {
            return GetNullable<bool>(index, CastToBoolean);
        }

        public Nullable<bool> GetNullableBoolean(string name) {
            return this.GetNullableBoolean(this.GetOrdinal(name));
        }

        public override byte GetByte(int i) {
            return this._reader.GetByte(i);
        }

        public byte GetByte(string name) {
            return this.GetByte(this.GetOrdinal(name));
        }

        public Nullable<byte> GetNullableByte(int index) {
            return GetNullable<byte>(index, CastToByte);
        }

        public Nullable<byte> GetNullableByte(string name) {
            return this.GetNullableByte(this.GetOrdinal(name));
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
            return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public override char GetChar(int i) {
            return this._reader.GetChar(i);
        }

        public char GetChar(string name) {
            return this.GetChar(this.GetOrdinal(name));
        }

        public Nullable<char> GetNullableChar(int index) {
            return GetNullable<char>(index, CastToChar);
        }

        public Nullable<char> GetNullableChar(string name) {
            return this.GetNullableChar(this.GetOrdinal(name));
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
            return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public new IDataReader GetData(int i) {
            return this._reader.GetData(i);
        }

        public override string GetDataTypeName(int i) {
            return this._reader.GetDataTypeName(i);
        }

        public string GetDataTypeName(string name) {
            return this._reader.GetDataTypeName(this.GetOrdinal(name));
        }



        public DateTime GetDateTime(string name) {
            return this._reader.GetDateTime(this.GetOrdinal(name));
        }
        public override DateTime GetDateTime(int i) {
            return this._reader.GetDateTime(i);
        }


        public override double GetDouble(int i) {
            return this._reader.GetDouble(i);
        }

        public double GetDouble(string name) {
            return this._reader.GetDouble(this.GetOrdinal(name));
        }

        public Nullable<double> GetNullableDouble(int index) {
            return GetNullable<double>(index, CastToDouble);
        }

        public Nullable<double> GetNullableDouble(string name) {
            return this.GetNullableDouble(this.GetOrdinal(name));
        }

        public override Type GetFieldType(int i) {
            return this._reader.GetFieldType(i);
        }

        public Type GetFieldType(string name) {
            return this._reader.GetFieldType(this.GetOrdinal(name));
        }

        public override float GetFloat(int i) {
            return this._reader.GetFloat(i);
        }

        public float GetFloat(string name) {
            return this._reader.GetFloat(this.GetOrdinal(name));
        }

        public Nullable<float> GetNullableFloat(int index) {
            return GetNullable<float>(index, CastToFloat);
        }

        public Nullable<float> GetNullableFloat(string name) {
            return this.GetNullableFloat(this.GetOrdinal(name));
        }

        public override Guid GetGuid(int i) {
            return this._reader.GetGuid(i);
        }

        public Guid GetGuid(string name) {
            return this._reader.GetGuid(this.GetOrdinal(name));
        }

        public Nullable<Guid> GetNullableGuid(int index) {
            return GetNullable<Guid>(index, GetGuid);
        }

        public Nullable<Guid> GetNullableGuid(string name) {
            return this.GetNullableGuid(this.GetOrdinal(name));
        }

        public override short GetInt16(int i) {
            return this._reader.GetInt16(i);
        }

        public short GetInt16(string name) {
            return this._reader.GetInt16(this.GetOrdinal(name));
        }

        public Nullable<short> GetNullableInt16(int index) {
            return GetNullable<short>(index, CastToInt16);
        }

        public Nullable<short> GetNullableInt16(string name) {
            return this.GetNullableInt16(this.GetOrdinal(name));
        }

        public override int GetInt32(int i) {
            return this._reader.GetInt32(i);
        }

        public int GetInt32(string name) {
            return this._reader.GetInt32(this.GetOrdinal(name));
        }

        public int? GetNullableInt(string name) {
            return this.GetNullable<int>(this.GetOrdinal(name), CastToInt32);
        }

        public string GetNullableString(string name) {
            var obj = this.GetValue(name);
            return (obj == null) ? null : Convert.ToString(obj);
            //return this.GetNullable<string>(this.GetOrdinal(name), CastToString);
        }
        public string GetNullableString(int i) {
            return this.CastToString(i);
            //return this.GetNullable<string>(i, CastToString);
        }

        public Nullable<int> GetNullableInt32(int index) {
            return GetNullable<int>(index, CastToInt32);
        }

        public Nullable<int> GetNullableInt32(string name) {
            return this.GetNullableInt32(this.GetOrdinal(name));
        }

        public override long GetInt64(int i) {
            return this._reader.GetInt64(i);
        }

        public long GetInt64(string name) {
            return this._reader.GetInt64(this.GetOrdinal(name));
        }

        public override decimal GetDecimal(int i) {
            return this._reader.GetDecimal(i);
        }

        public decimal GetDecimal(string name) {
            return this._reader.GetDecimal(this.GetOrdinal(name));
        }

        public Nullable<Decimal> GetNullableDecimal(int index) {
            return GetNullable<decimal>(index, CastToDecimal);
        }

        public Nullable<Decimal> GetNullableDecimal(string name) {
            return this.GetNullableDecimal(this.GetOrdinal(name));
        }


        public Nullable<long> GetNullableInt64(int index) {
            return GetNullable<long>(index, CastToInt64);
        }

        public Nullable<long> GetNullableInt64(string name) {
            return this.GetNullableInt64(this.GetOrdinal(name));
        }


        public Nullable<DateTime> GetNullableDateTime(int index) {
            return GetNullable<DateTime>(index, CastToDateTime);
        }

        public Nullable<DateTime> GetNullableDateTime(string name) {
            return this.GetNullableDateTime(this.GetOrdinal(name));
        }


        public override string GetName(int i) {
            return this._reader.GetName(i);
        }

        public override int GetOrdinal(string name) {
            if (!this._columns.ContainsKey(name))
                throw new KeyNotFoundException("Failed to find Ordinal from Data Reader. Column Name: " + name + " doesn't exists");

            return this._columns[name];
        }

        public override string GetString(int i) {
            return this._reader.GetString(i);
        }

        public string GetString(string name) {
            return this._reader.GetString(this.GetOrdinal(name));
        }


        public List<string> Columns {
            get {
                return (from k in this._columns.Keys select k).ToList();
            }
        }
        //public string GetNullableString(int index) {
        //    string nullable;
        //    if (this._reader.IsDBNull(index)) {
        //        nullable = null;
        //    } else {
        //        nullable = this._reader.GetString(index);
        //    }
        //    return nullable;
        //}

        //public string GetNullableString(string name) {
        //    return this.GetNullableString(this.GetOrdinal(name));
        //}

        public override object GetValue(int i) {
            return this._reader.GetValue(i);
        }

        public object GetValue(string name) {
            return this._reader.GetValue(this.GetOrdinal(name));
        }

        public override int GetValues(object[] values) {
            return this._reader.GetValues(values);
        }

        private Nullable<T> GetNullable<T>(int ordinal, Conversion<T> convert) where T : struct {
            Nullable<T> nullable;
            if (_reader.IsDBNull(ordinal)) {
                nullable = null;
            } else {
                nullable = convert(ordinal);
            }
            return nullable;
        }

        #region -------- CastTo Methods --------
        public bool CastToBoolean(string name) {
            return Convert.ToBoolean(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public bool CastToBoolean(int i) {
            return Convert.ToBoolean(this._reader.GetValue(i));
        }

        public byte CastToByte(string name) {
            return Convert.ToByte(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public byte CastToByte(int i) {
            return Convert.ToByte(this._reader.GetValue(i));
        }

        public char CastToChar(string name) {
            return Convert.ToChar(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public char CastToChar(int i) {
            return Convert.ToChar(this._reader.GetValue(i));
        }

        public DateTime CastToDateTime(string name) {
            return Convert.ToDateTime(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public DateTime CastToDateTime(int i) {
            return Convert.ToDateTime(this._reader.GetValue(i));
        }

        public decimal CastToDecimal(string name) {
            return Convert.ToDecimal(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public decimal CastToDecimal(int i) {
            return Convert.ToDecimal(this._reader.GetValue(i));
        }

        public double CastToDouble(string name) {
            return Convert.ToDouble(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public double CastToDouble(int i) {
            return Convert.ToDouble(this._reader.GetValue(i));
        }

        public float CastToFloat(string name) {
            return Convert.ToSingle(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public float CastToFloat(int i) {
            return Convert.ToSingle(this._reader.GetValue(i));
        }

        public short CastToInt16(string name) {
            return Convert.ToInt16(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public short CastToInt16(int i) {
            return Convert.ToInt16(this._reader.GetValue(i));
        }

        public int CastToInt32(string name) {
            return Convert.ToInt32(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public int CastToInt32(int i) {
            return Convert.ToInt32(this._reader.GetValue(i));
        }

        public long CastToInt64(string name) {
            return Convert.ToInt64(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public long CastToInt64(int i) {
            return Convert.ToInt64(this._reader.GetValue(i));
        }


        public string CastToString(string name) {
            return Convert.ToString(this._reader.GetValue(this.GetOrdinal(name)));
        }

        public string CastToString(int i) {
            return Convert.ToString(this._reader.GetValue(i));
        }
        #endregion


        #region -------- PUBLIC OVERRIDES --------
        public override void Close() {
            this._reader.Close();
        }

        public override int Depth {
            get { return this._reader.Depth; }
        }

        public override DataTable GetSchemaTable() {
            return this._reader.GetSchemaTable();
        }

        public override bool IsClosed {
            get { return this._reader.IsClosed; }
        }

        public override bool NextResult() {
            return this._reader.NextResult();
        }



        public override bool Read() {
            if (this._aborted)
                return false;
            return this._reader.Read();
        }

        public override int RecordsAffected {
            get { return this._reader.RecordsAffected; }
        }

        public override IEnumerator GetEnumerator() {
            return new DbEnumerator(this._reader, this._reader.IsClosed);
        }

        public override object this[string name] {
            get { return this._reader[name]; }
        }

        public override object this[int i] {
            get { return this._reader[i]; }
        }

        public override bool IsDBNull(int i) {
            return this._reader.IsDBNull(i);
        }

        public bool IsDBNull(string name) {
            return this._reader.IsDBNull(this.GetOrdinal(name));
        }

        public override int FieldCount {
            get { return this._reader.FieldCount; }
        }

        public override bool HasRows {
            get { return this._reader.HasRows; }
        }
        #endregion
    }
}
