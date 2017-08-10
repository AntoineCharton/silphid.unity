﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Silphid.Extensions;
using UnityEditor;
using UnityEngine;

namespace Silphid.Showzup
{
    public class ManifestWindow : EditorWindow
    {
        private GUIStyle _normalStyle;
        private GUIStyle _mappingArrowStyle;
        private GUIStyle _explicitVariantStyle;
        private GUIStyle _implicitVariantStyle;
        private string _filter = "";
        private Vector2 _scrollPos;

        [MenuItem("Silphid/Showzup/Manifest Window")]
        public static void Open()
        {
            GetWindow(typeof(ManifestWindow));
        }
        
        [UsedImplicitly]
        public void OnEnable()
        {
            _normalStyle = new GUIStyle(EditorStyles.boldLabel);
            _normalStyle.stretchWidth = false;
            _normalStyle.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.69f, 0.69f, 0.69f)
                : new Color(0.28f, 0.28f, 0.28f);

            _mappingArrowStyle = new GUIStyle(EditorStyles.boldLabel);
            _mappingArrowStyle.stretchWidth = false;
            _mappingArrowStyle.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.83f, 0.68f, 0.52f)
                : new Color(0.6f, 0.49f, 0.37f);

            _explicitVariantStyle = new GUIStyle(EditorStyles.boldLabel);
            _explicitVariantStyle.stretchWidth = false;
            _explicitVariantStyle.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.95f, 0.54f, 0f)
                : new Color(0.67f, 0.35f, 0f);

            _implicitVariantStyle = new GUIStyle(EditorStyles.boldLabel);
            _implicitVariantStyle.stretchWidth = false;
            _implicitVariantStyle.normal.textColor = EditorGUIUtility.isProSkin
                ? new Color(0.62f, 0.31f, 0f)
                : new Color(0.4f, 0.2f, 0f);
        }
        
        public void OnGUI()
        {
            var manifest = ManifestManager.Manifest;

            // Begin scroll
            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            // Build button
            if (GUILayout.Button("Build", GUILayout.ExpandWidth(true)))
                ManifestBuilder.Build();

            // Filter
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Filter", EditorStyles.boldLabel);
            _filter = GUILayout.TextField(_filter, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            
            // Models => View Models
            EditorGUILayout.Separator();
            GUILayout.Label("Models => View Models", EditorStyles.whiteLargeLabel);
            Labels(manifest.ModelsToViewModels);
            
            // View Models => Views
            EditorGUILayout.Separator();
            GUILayout.Label("View Models => Views", EditorStyles.whiteLargeLabel);
            Labels(manifest.ViewModelsToViews);
            
            // Views => Prefabs
            EditorGUILayout.Separator();
            GUILayout.Label("Views => Prefabs", EditorStyles.whiteLargeLabel);
            Labels(manifest.ViewsToPrefabs);
            
            // End scroll
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void Labels(IEnumerable<object> objects)
        {
            objects
                .Select(x => new { Obj = x, Str = x.ToString() })
                .Where(x => _filter.IsNullOrEmpty() || x.Str.CaseInsensitiveContains(_filter))
                .OrderBy(x => x.Str)
                .ForEach(x => Label(x.Obj, x.Str));
        }

        private void Label(object obj, string str)
        {
            if (obj is TypeToTypeMapping)
                Label((TypeToTypeMapping) obj);
            else
                Label((TypeToUriMapping) obj);
        }

        private void Label(TypeToTypeMapping mapping)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(mapping.Source.Name, _normalStyle);
            GUILayout.Label(" => ", _mappingArrowStyle);
            GUILayout.Label(mapping.Target.Name, _normalStyle);
            
            if (mapping.Variants.Any())
                GUILayout.Label($" [{mapping.Variants}]", _explicitVariantStyle);
            
            if (mapping.ImplicitVariants.Any())
                GUILayout.Label($" ({mapping.ImplicitVariants})", _implicitVariantStyle);

            EditorGUILayout.EndHorizontal();
        }

        private void Label(TypeToUriMapping mapping)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(mapping.Source.Name, _normalStyle);
            GUILayout.Label(" => ", _mappingArrowStyle);
            GUILayout.Label(mapping.Target.ToString(), _normalStyle);
            
            if (mapping.Variants.Any())
                GUILayout.Label($" [{mapping.Variants}]", _explicitVariantStyle);

            EditorGUILayout.EndHorizontal();
        }
    }
}