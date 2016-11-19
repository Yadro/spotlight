using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace spotlight
{
    public class SearchRangeManager
    {
        public class Choise
        {
            public int Count;
            public string ChoiseResult;

            public Choise(int count, string choiseResult = "")
            {
                ChoiseResult = choiseResult;
                Count = count;
            }

            public override string ToString()
            {
                return $"{Count} - \"{ChoiseResult}\"";
            }
        }

        public class ChoiseInf
        {
            public List<Choise> Choises;
            public int Count = 1;

            public ChoiseInf(List<Choise> choises)
            {
                Choises = choises;
            }

            public void AddChoise(string selectedResult)
            {
                var choise = Choises.Find(el => el.ChoiseResult == selectedResult);
                if (choise != null)
                    choise.Count++;
                else
                    Choises.Add(new Choise(1, selectedResult));
                Count++;
            }
        }

        public Dictionary<string, int> QuerysDict = new Dictionary<string, int>();
        public Dictionary<string, List<Choise>> ChooseResult;
        private const string ChoisesFileName = "choises.json";

        public SearchRangeManager()
        {
            this.ChooseResult = DeserializeData(ReadFile(ChoisesFileName)) ?? new Dictionary<string, List<Choise>>();
        }

        public void AddQuery(string query, string selectedResult = "")
        {
            if (this.ChooseResult.ContainsKey(query))
            {
                List<Choise> choises = this.ChooseResult[query];
                var choise = choises.Find(el => el.ChoiseResult == selectedResult);
                if (choise != null)
                    choise.Count++;
                else
                    choises.Add(new Choise(1, selectedResult));
            }
            else
            {
                var choice = new List<Choise>();
                if (selectedResult != "")
                    choice.Add(new Choise(1, selectedResult));

                this.ChooseResult.Add(query, choice);
            }
        }

        public void AddQuery(string query)
        {
            if (this.QuerysDict.ContainsKey(query))
                this.QuerysDict[query]++;
            else
                this.QuerysDict.Add(query, 1);
        }

        public int GetQueryCount(string query)
        {
            return this.QuerysDict.ContainsKey(query) ? this.QuerysDict[query] : 0;
        }

        private Dictionary<string, int> StrToQuery(string[] queries)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var query in queries)
            {
                var items = query.Split(';');
                dictionary.Add(items[0], int.Parse(items[1]));
            }
            return dictionary;
        }

        private string QueryToStr(Dictionary<string, int> queries)
        {
            var res = queries.Select(query => $"{query.Key};{query.Value}");
            return string.Join("\n", res);
        }

        public void DoSerializeData()
        {
            WriteFile(ChoisesFileName, SerializeData(this.ChooseResult));
        }

        private string SerializeData(Dictionary<string, List<Choise>> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }

        private Dictionary<string, List<Choise>> DeserializeData(string data)
        {
            var result = new Dictionary<string, List<Choise>>();

            JObject obj = JObject.Parse(data);
            foreach (KeyValuePair<string, JToken> pair in obj)
            {
                List<Choise> choises = new List<Choise>();
                List<JToken> listToken = pair.Value.ToList();
                foreach (JToken jToken in listToken)
                {
                    choises.Add(jToken.ToObject<Choise>());
                }
                result.Add(pair.Key, choises);
            }
            return result;
        }

        private string ReadFile(string file)
        {
            try
            {
                return System.IO.File.ReadAllText(file);
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private void WriteFile(string file, string content)
        {
            System.IO.File.WriteAllText(file, content);
        }
    }
}