﻿using Cake.Core;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Cake.VSProjectProperty
{
    /// <summary>
    /// The Project File info helper.
    /// </summary>
    public sealed class ProjectFileHelper
    {
        private string _path = null;
        private XmlDocument _doc = null;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <param name="configure"></param>
        public ProjectFileHelper(string projectFilePath)
        {
            if (!System.IO.File.Exists(projectFilePath)) throw new CakeException("Project file not exist.");
            _path = projectFilePath;

            _doc = new XmlDocument();
            _doc.Load(_path);
        }

        private bool IsTargetGroup(XmlNode group, string config, string platform)
        {
            if (group.Attributes.Count == 0)
            {
                if (group.ChildNodes[0].Name != "Configuration") return false;
                if (!group.ChildNodes[0].InnerText.ToLower().Contains(config)) return false;
                if (group.ChildNodes[1].Name != "Platform") return false;
                if (!group.ChildNodes[1].InnerText.ToLower().Contains(platform)) return false;
                return true;
            }

            foreach (XmlAttribute attri in group.Attributes)
            {
                if (attri.Name != "Condition") continue;
                if (!attri.InnerText.ToLower().Contains(config)) continue;
                if (!attri.InnerText.ToLower().Contains(platform)) continue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="config"></param>
        ///   /// <param name="platform"></param>
        public void SetProperty(string key, string value, string config = "release", string platform = "anycpu")
        {
            XmlNode root = _doc.DocumentElement;
            if (root == null || root.Name != "Project") throw new CakeException("not a valid Project file.");
          

            foreach (XmlNode group in root.ChildNodes)
            {
                if (group.Name != "PropertyGroup" && group.Name != "ItemDefinitionGroup") continue;
                if (!IsTargetGroup(group, config, platform)) continue;

                foreach (XmlNode item in group.ChildNodes)
                {
                    if (item.ChildNodes.Count == 1)
                    {
                        if (item.Name != key) continue;
                        item.InnerText = value;
                        return;
                    }

                    foreach (XmlNode item2 in item.ChildNodes)
                    {
                        if (item2.Name != key) continue;
                        item2.InnerText = value;
                        return;
                    }
                 }
            }
        }




        /// <summary>
        ///  GetProperty
        /// </summary>
        /// <param name="key"></param>
        /// <param name="config"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public string GetProperty(string key, string config = "release", string platform = "anycpu")
        {
            XmlNode root = _doc.DocumentElement;
            if (root == null || root.Name != "Project") throw new CakeException("Project file is not a valid .csproj file.");

            foreach (XmlNode group in root.ChildNodes)
            {
                if (group.Name != "PropertyGroup" && group.Name != "ItemDefinitionGroup") continue;
                if (!IsTargetGroup(group, config, platform)) continue;

                foreach (XmlNode item in group.ChildNodes)
                {
                    if (item.ChildNodes.Count == 1)
                    {
                        if (item.Name != key) continue;
                        return item.InnerText;
                    }

                    foreach (XmlNode item2 in item.ChildNodes)
                    {
                        if (item2.Name != key) continue;
                        return item2.InnerText;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            _doc.Save(_path);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reload()
        {
            _doc = null;
            _doc = new XmlDocument();
            _doc.Load(_path);
        }
    }


}
