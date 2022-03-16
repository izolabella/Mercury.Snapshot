﻿
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Mercury.Snapshot.Objects.Structures.Financial;

namespace Mercury.Snapshot.Objects.Util
{
    public class GoogleSheetsManager
    {
        public GoogleSheetsManager(UserCredential Credential)
        {
            this.Service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credential,
                ApplicationName = GoogleClient.ApplicationName,
            });
        }


        private SheetsService Service { get; set; }


        public decimal? GetUserBalance(string SpreadsheetId)
        {
            SpreadsheetsResource.ValuesResource.GetRequest GetCells = this.Service.Spreadsheets.Values.Get(SpreadsheetId, "Expenditure!G2");
            ValueRange Response = GetCells.Execute();
            IList<IList<object>> Values = Response.Values;
            if (Values != null && Values.Count > 0 && decimal.TryParse(((string)Values[0][0]).Remove(((string)Values[0][0]).LastIndexOf('$'), 1), out decimal Amount))
                return Amount;
            return null;
        }


        public IReadOnlyList<Expenditure> GetUserExpenditures(string SpreadsheetId)
        {
            SpreadsheetsResource.ValuesResource.GetRequest GetCells = this.Service.Spreadsheets.Values.Get(SpreadsheetId, "Expenditure!A:D");
            ValueRange Response = GetCells.Execute();
            IList<IList<object>> Values = Response.Values;
            List<Expenditure> Expenditures = new();
            if (Values != null && Values.Count > 0)
                foreach (IList<object> Row in Values)
                    if (DateTime.TryParse((string)Row[0], out DateTime Timestamp) && decimal.TryParse(((string)Row[1]).Remove(((string)Row[1]).LastIndexOf('$'), 1), out decimal Amount))
                        Expenditures.Add(new(Timestamp, Amount, (string)Row[2], (string)Row[3]));
            return Expenditures;
        }
    }
}