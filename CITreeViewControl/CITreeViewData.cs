using System;
using System.Collections.Generic;

namespace CITreeViewControl
{
    public class CITreeViewData
    {

        public string Name { get; set; }
        public List<CITreeViewData> Children { get; set; }

        public CITreeViewData()
        {
            this.Name = String.Empty;
            this.Children = new List<CITreeViewData>();
        }
        public CITreeViewData(string name)
        {
            this.Name = name;
            this.Children = new List<CITreeViewData>();
        }
        public CITreeViewData(string name, List<CITreeViewData> children)
        {
            this.Name = name;
            this.Children = children;
        }
        void AddChild(CITreeViewData child)
        {
            this.Children.Add(child);
        }
        void RemoveChild(CITreeViewData child)
        {
            this.Children.Remove(child);
        }

        public static List<CITreeViewData> GetDefaultData()
        {
            var subChild121 = new CITreeViewData(name: "Albea");
            var subChild122 = new CITreeViewData(name: "Egea");
            var subChild123 = new CITreeViewData(name: "Linea");
            var subChild124 = new CITreeViewData(name: "Siena");

            var child11 = new CITreeViewData(name: "Volvo");
            var child12 = new CITreeViewData(name: "Fiat", children: new List<CITreeViewData> { subChild121, subChild122, subChild123, subChild124 });
            var child13 = new CITreeViewData(name: "Alfa Romeo");
            var child14 = new CITreeViewData(name: "Mercedes");
            var parent1 = new CITreeViewData(name: "Sedan", children: new List<CITreeViewData> { child11, child12, child13, child14 });

            var subChild221 = new CITreeViewData(name: "Discovery");
            var subChild222 = new CITreeViewData(name: "Evoque");
            var subChild223 = new CITreeViewData(name: "Defender");
            var subChild224 = new CITreeViewData(name: "Freelander");


            var child21 = new CITreeViewData(name: "GMC");
            var child22 = new CITreeViewData(name: "Land Rover", children: new List<CITreeViewData> { subChild221, subChild222, subChild223, subChild224 });
            var parent2 = new CITreeViewData(name: "SUV", children: new List<CITreeViewData> { child21, child22 });



            var child31 = new CITreeViewData(name: "Wolkswagen");
            var child32 = new CITreeViewData(name: "Toyota");
            var child33 = new CITreeViewData(name: "Dodge");
            var parent3 = new CITreeViewData(name: "Truck", children: new List<CITreeViewData> { child31, child32, child33 });


            var subChildChild5321 = new CITreeViewData(name: "Carrera", children: new List<CITreeViewData> { child31, child32, child33 });
            var subChildChild5322 = new CITreeViewData(name: "Carrera 4 GTS");
            var subChildChild5323 = new CITreeViewData(name: "Targa 4");
            var subChildChild5324 = new CITreeViewData(name: "Turbo S");


            var parent4 = new CITreeViewData(name: "Van", children: new List<CITreeViewData> { subChildChild5321, subChildChild5322, subChildChild5323, subChildChild5324 });




            var subChild531 = new CITreeViewData(name: "Cayman");
            var subChild532 = new CITreeViewData(name: "911", children: new List<CITreeViewData> { subChildChild5321, subChildChild5322, subChildChild5323, subChildChild5324 });


            var child51 = new CITreeViewData(name: "Renault");
            var child52 = new CITreeViewData(name: "Ferrari");
            var child53 = new CITreeViewData(name: "Porshe", children: new List<CITreeViewData> { subChild531, subChild532 });
            var child54 = new CITreeViewData(name: "Maserati");
            var child55 = new CITreeViewData(name: "Bugatti");
            var parent5 = new CITreeViewData(name: "Sports Car", children: new List<CITreeViewData> { child51, child52, child53, child54, child55 });

            return new List<CITreeViewData>() { parent5, parent2, parent1, parent3, parent4 };
        }


    }
}
