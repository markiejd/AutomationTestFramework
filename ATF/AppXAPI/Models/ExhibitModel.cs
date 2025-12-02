using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{
    public class ExhibitModel
    {
        public string? seizedLocation { get; set; }
        public string exhibitType { get; set; } = string.Empty;
        public string? currency { get; set; }    
        public bool noBag { get; set; } = false;
        public string? innerBag { get; set; }
        public string? outerBag { get; set; }
        public string seizedBy { get; set; } = string.Empty;
        public bool retained { get; set; } = false;
        public string dateSeized { get; set; } = string.Empty;
        public string timeSeized { get; set; } = string.Empty;
        public string detailedDescriptionOfTheExhibit { get; set; } = string.Empty;
        public string whereSeized { get; set; } = string.Empty;
        public string exhibitReferenceNumber { get; set; } = string.Empty;
        public bool additionalPowerToSeize { get; set; } = false;
        public string? additionalPower { get; set; }
        public string? enterAdditionalPower { get; set; }
        public string additionalInformation { get; set; } = string.Empty;
    }

    public class GetExhibits
    {
        public static ExhibitModel CreateExhibit(string? seizedLocation, string exhibitType, string? currency, bool noBag, string? innerBag, 
            string? outerBag, string seizedBy, bool retained, string dateSeized, string timeSeized, string detailedDescriptionOfTheExhibit,
             string whereSeized, string exhibitReferenceNumber, bool additionalPowerToSeize, string additionalPower, string enterAdditionalPower, 
             string additionalInformation)
        {
            ExhibitModel model = new ExhibitModel();
            model.seizedLocation = seizedLocation;
            model.exhibitType = exhibitType;
            model.currency = currency;
            model.noBag = noBag;
            model.innerBag = innerBag;
            model.outerBag = outerBag;
            model.seizedBy = seizedBy;
            model.retained = retained;
            model.dateSeized = dateSeized;
            model.timeSeized = timeSeized;
            model.detailedDescriptionOfTheExhibit = detailedDescriptionOfTheExhibit;
            model.whereSeized = whereSeized;
            model.exhibitReferenceNumber = exhibitReferenceNumber;
            model.additionalPowerToSeize = additionalPowerToSeize;
            model.additionalPower = additionalPower;
            model.enterAdditionalPower = enterAdditionalPower;
            model.additionalInformation = additionalInformation;

            return model;
        }

        public static ExhibitModel GetExhibit(string ern)
        {
            ExhibitModel model = new ExhibitModel();
            switch (ern.ToLower())
            {
                default:
                    {
                        model.seizedLocation = null;
                        model.exhibitType = "Fax";
                        model.currency = null;
                        model.noBag = true;
                        model.innerBag = null;
                        model.outerBag = null;
                        model.seizedBy = "COOPER, Fred, 20017 (FC)";
                        model.retained = false;
                        model.dateSeized = "TODAY";
                        model.timeSeized = "02:00";
                        model.detailedDescriptionOfTheExhibit = "description here";
                        model.whereSeized = "under the sofa";
                        model.exhibitReferenceNumber = ern;
                        model.additionalPowerToSeize = false;
                        model.additionalPower = null;
                        model.enterAdditionalPower = null;
                        model.additionalInformation = "additional information goes in here";
                        break;
                    }         
                case "fc/exhibit/epoch/01":
                    {
                        model.seizedLocation = null;
                        model.exhibitType = "Fax";
                        model.currency = null;
                        model.noBag = false;
                        model.innerBag = null;
                        model.outerBag = "OU-EPOCH";
                        model.seizedBy = "COOPER, Fred, 20017 (FC)";
                        model.retained = false;
                        model.dateSeized = "TODAY";
                        model.timeSeized = "02:00";
                        model.detailedDescriptionOfTheExhibit = "description here";
                        model.whereSeized = "under the sofa";
                        model.exhibitReferenceNumber = ern;
                        model.additionalPowerToSeize = false;
                        model.additionalPower = null;
                        model.enterAdditionalPower = null;
                        model.additionalInformation = "put in a bag!";
                        break;
                    }           
            }
            return model;
        }
    }


}
