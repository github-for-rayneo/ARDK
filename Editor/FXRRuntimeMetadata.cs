#if XR_MGMT_GTE_320

namespace FFalcon.XR.Runtime.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.XR.Management.Metadata;
    using UnityEngine;

    public class FXRRuntimePackage : IXRPackage
    {
        public IXRPackageMetadata metadata => new PackageMetadata();

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            return true;
        }

        private class LoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName => "RayNeo";

            public string loaderType => typeof(Runtime.XRLoader).FullName;

            public List<BuildTargetGroup> supportedBuildTargets => new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS
            };
        }

        private class PackageMetadata : IXRPackageMetadata
        {
            public string packageName => "RayNeo";

            public string packageId => "com.rayneo.ardk";

            public string settingsType => typeof(Runtime.XRSettings).FullName;

            public List<IXRLoaderMetadata> loaderMetadata => new List<IXRLoaderMetadata>()
            {
                new LoaderMetadata()
            };
        }
    }
}

#endif // XR_MGMT_GTE_320