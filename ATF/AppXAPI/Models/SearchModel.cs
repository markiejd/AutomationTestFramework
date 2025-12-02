using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{
    public class SearchModel
    {
        public string searchOfficers { get; set; } = String.Empty;
        public string operation { get; set; } = String.Empty;
        public string sseo { get; set; } = String.Empty;
        public string ssrn { get; set; } = String.Empty;
        public string warrantType { get; set; } = String.Empty;
        public string? section { get; set; }
        public string? actSection { get; set; }
        public string warrantDate { get; set; } = String.Empty;
        public string sceneType { get; set; } = String.Empty;
        public string address1 { get; set; } = String.Empty;
        public string? address2 { get; set; } = String.Empty;
        public string town { get; set; } = String.Empty;
        public string? county { get; set; }
        public string postcode { get; set; } = String.Empty;
        public string searchStartDate { get; set; } = String.Empty;
        public string searchStartTime { get; set; } = String.Empty;
        public string? additionalInformation { get; set; }

    }

    public class GetSearches
    {
        public static SearchModel CreateModel(string officerList, string operation, string sseo, string ssrn, string warrantType, string? section, string? actSection, string warrantDate, string sceneType, string add1, string? add2, string town, string? county, string postCode, string startDate, string startTime, string? addInfo)
        {
            SearchModel returnSearch = new SearchModel();
            returnSearch.searchOfficers = officerList;
            returnSearch.operation = operation;
            returnSearch.sseo = sseo;
            returnSearch.ssrn = ssrn;
            returnSearch.warrantType = warrantType;
            returnSearch.section = section;
            returnSearch.actSection = actSection;
            returnSearch.warrantDate = warrantDate;
            returnSearch.sceneType = sceneType;
            returnSearch.address1 = add1;
            returnSearch.address2 = add2;
            returnSearch.town = town;
            returnSearch.county = county;
            returnSearch.postcode = postCode;
            returnSearch.searchStartDate = startDate;
            returnSearch.searchStartTime = startTime;
            returnSearch.additionalInformation = addInfo;
            return returnSearch;
        }

        public static SearchModel GetSearch(string ssrn)
        {
            SearchModel returnSearch = new SearchModel();
            switch (ssrn.ToLower())
            {
                default:
                    {
                        returnSearch.searchOfficers = "COOPER, Fred, 20017 (FC)";
                        returnSearch.operation = "Titan";
                        returnSearch.sseo = "COOPER, Fred, 20017 (FC)";
                        returnSearch.ssrn = ssrn;
                        returnSearch.warrantType = "Warrant";
                        returnSearch.section = null;
                        returnSearch.actSection = null;
                        returnSearch.warrantDate = "FIRSTOFTHEMONTH";
                        returnSearch.sceneType = "Premise or Locaiton";
                        returnSearch.address1 = "Address Line 1";
                        returnSearch.address2 = null;
                        returnSearch.town = "Aberdeen";
                        returnSearch.county = null;
                        returnSearch.postcode = "AB11 1AA";
                        returnSearch.searchStartDate = "TODAY";
                        returnSearch.searchStartTime = "01:00";
                        returnSearch.additionalInformation = null;
                        break;
                    }
                case "fc/epoch/01":
                    {
                        returnSearch.searchOfficers = "BOWEN, Romeo, 43243 (RB)|COOPER, Fred, 20017 (FC)";
                        returnSearch.operation = "Titan";
                        returnSearch.sseo = "COOPER, Fred, 20017 (FC)";
                        returnSearch.ssrn = "FC/EPOCH/01";
                        returnSearch.warrantType = "Warrant";
                        returnSearch.section = null;
                        returnSearch.actSection = null;
                        returnSearch.warrantDate = "FIRSTOFTHEMONTH";
                        returnSearch.sceneType = "Premise or Location";
                        returnSearch.address1 = "Address Line 1";
                        returnSearch.address2 = "Address Line 2";
                        returnSearch.town = "Aberdeen";
                        returnSearch.county = "Aberdeenshire";
                        returnSearch.postcode = "AB11 1AA";
                        returnSearch.searchStartDate = "TODAY";
                        returnSearch.searchStartTime = "01:00";
                        returnSearch.additionalInformation = "I put additional information about this search HERE!";
                        break;
                    }
            }            
            return returnSearch;
        }
    }
}
