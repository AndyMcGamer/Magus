using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Magus.Skills.SkillTree
{
    [CreateAssetMenu(fileName = "New SkillDatabase", menuName = "Magus/Skills/SkillDatabase")]
    public class SkillDatabase : ScriptableObject
    {
        public HashSet<SkillNode> skills;
        [Expandable, ReorderableList, SerializeField] private List<SkillNode> _skills;

        public void Initialize()
        {
            skills = _skills.ToHashSet();
            foreach (var skillNode in skills)
            {
                skillNode.postrequisites = new();
                foreach (var prereq in skillNode.prerequisites)
                {
                    SkillNode sn = skills.First(x => x.skillData == prereq.skill);
                    string skillName = skillNode.skillData.name;
                    sn.postrequisites.Add(skillName);
                }
            }
        }

        public T FindDataByName<T>(string name) where T : SkillData
        {
            foreach (var skill in skills)
            {
                if(skill.skillData.Name == name)
                {
                    return skill.skillData as T;
                }
            }
            return null;
        }

        public SkillNode FindNodeByName(string skillName)
        {
            return skills.FirstOrDefault(x => x.skillData.Name == skillName);
        }

#if UNITY_EDITOR
        private readonly string subFolderName = "/Prerequisites";

        [Button("Create SkillNode")]
        private void CreateNode()
        {
            var prerequisiteDirectory = new DirectoryInfo(Directory.GetParent(AssetDatabase.GetAssetPath(this)) + subFolderName);
            if (!prerequisiteDirectory.Exists)
            {
                prerequisiteDirectory.Create();
            }
            var newNode = ScriptableObject.CreateInstance<SkillNode>();
            string assetPath = prerequisiteDirectory + "/New SkillNode.asset";
            string name = AssetDatabase.GenerateUniqueAssetPath(assetPath[assetPath.IndexOf("Assets")..]);

            foreach (var node in _skills)
            {
                if (node.skillData == newNode.skillData) return;
            }

            _skills.Add(newNode);

            AssetDatabase.CreateAsset(newNode, name);
            AssetDatabase.SaveAssets();

        }

        [Button("Clear Unused Nodes")]
        private void ClearUnused()
        {
            var prerequisiteDirectory = new DirectoryInfo(Directory.GetParent(AssetDatabase.GetAssetPath(this)) + subFolderName);
            if (!prerequisiteDirectory.Exists) return;
            string[] files = Directory.GetFiles(prerequisiteDirectory.ToString(), "*.asset", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var node = AssetDatabase.LoadAssetAtPath(file[file.IndexOf("Assets")..], typeof(SkillNode));
                if(node != null && !skills.Contains(node))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
                }
            }
            if(Directory.GetFiles(prerequisiteDirectory.ToString()).Length == 0)
            {
                string path = prerequisiteDirectory.ToString();
                AssetDatabase.DeleteAsset(path[path.IndexOf("Assets")..]);
            }
            AssetDatabase.SaveAssets();
        }

        [Button("Rebuild Postrequisites")]
        private void RebuildPostRequisites()
        {
            foreach (var skillNode in skills)
            {
                skillNode.postrequisites = new();
                foreach (var prereq in skillNode.prerequisites)
                {
                    SkillNode sn = skills.First(x => x.skillData == prereq.skill);
                    string skillName = skillNode.skillData.name;
                    sn.postrequisites.Add(skillName);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private void OnValidate()
        {
            skills = _skills.ToHashSet();
            //RebuildPostRequisites();
        }
#endif
    }
}
