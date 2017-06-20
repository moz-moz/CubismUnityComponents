﻿/*
 * Copyright(c) Live2D Inc. All rights reserved.
 * 
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http://live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace Live2D.Cubism.Core
{
    /// <summary>
    /// Extension for Cubism related arrays.
    /// </summary>
    public static class ArrayExtensionMethods
    {
        #region Parameters

        /// <summary>
        /// Finds a <see cref="CubismParameter"/> by its ID.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Parameter on success; <see langword="null"/> otherwise.</returns>
        public static CubismParameter FindById(this CubismParameter[] self, string id)
        {
            return (self != null)
                ? Array.Find(self, i => i.name == id)
                : null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismParameter"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismParameter[] self, IntPtr model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static unsafe void WriteTo(this CubismParameter[] self, IntPtr unmanagedModel)
        {
            // Get address.
            var values = csmGetParameterValues(unmanagedModel);


            // Push.
            for (var i = 0; i < self.Length; ++i)
            {
                values[self[i].UnmanagedIndex] = self[i].Value;
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static unsafe void ReadFrom(this CubismParameter[] self, IntPtr unmanagedModel)
        {
            // Get address.
            var values = csmGetParameterValues(unmanagedModel);


            // Pull.
            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Value = values[self[i].UnmanagedIndex];
            }
        }

        #endregion

        #region Parts

        /// <summary>
        /// Finds a <see cref="CubismPart"/> by its ID.
        /// </summary>
        /// <param name="self"><see langword="this"/>.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Part if found; <see langword="null"/> otherwise.</returns>
        public static CubismPart FindById(this CubismPart[] self, string id)
        {
            return (self != null)
                ? Array.Find(self, i => i.name == id)
                : null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismPart"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismPart[] self, IntPtr model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }

        /// <summary>
        /// Writes opacities to unmanaged model.
        /// </summary>
        /// <param name="self">Source buffer.</param>
        /// <param name="unmanagedModel"></param>
        internal static unsafe void WriteTo(this CubismPart[] self, IntPtr unmanagedModel)
        {
            // Get address.
            var opacities = csmGetPartOpacities(unmanagedModel);


            // Push.
            for (var i = 0; i < self.Length; ++i)
            {
                opacities[self[i].UnmanagedIndex] = self[i].Opacity;
            }
        }

        #endregion

        #region Drawables

        /// <summary>
        /// Finds a <see cref="CubismParameter"/> by its ID.
        /// </summary>
        /// <param name="self"><see langword="this"/>.</param>
        /// <param name="id">ID to match.</param>
        /// <returns>Part if found; <see langword="null"/> otherwise.</returns>
        public static CubismDrawable FindById(this CubismDrawable[] self, string id)
        {
            return (self != null)
                ? Array.Find(self, i => i.name == id)
                : null;
        }


        /// <summary>
        /// Revives (and sorts) <see cref="CubismDrawable"/>s.
        /// </summary>
        /// <param name="self">Container.</param>
        /// <param name="model">TaskableModel to unmanaged model.</param>
        internal static void Revive(this CubismDrawable[] self, IntPtr model)
        {
            Array.Sort(self, (a, b) => a.UnmanagedIndex - b.UnmanagedIndex);


            for (var i = 0; i < self.Length; ++i)
            {
                self[i].Revive(model);
            }
        }


        /// <summary>
        /// Reads new data from a model.
        /// </summary>
        /// <param name="self">Buffer to write to.</param>
        /// <param name="unmanagedModel">Unmanaged model to read from.</param>
        internal static unsafe void ReadFrom(this CubismDynamicDrawableData[] self, IntPtr unmanagedModel)
        {
            // Get addresses.
            var flags = csmGetDrawableDynamicFlags(unmanagedModel);
            var opacities = csmGetDrawableOpacities(unmanagedModel);
            var drawOrders = csmGetDrawableDrawOrders(unmanagedModel);
            var renderOrders = csmGetDrawableRenderOrders(unmanagedModel);
            var vertexPositions = (Vector2**)csmGetDrawableVertexPositions(unmanagedModel).ToPointer();


            // Pull data.
            for (var i = 0; i < self.Length; ++i)
            {
                var data = self[i];


                data.Flags = flags[i];
                data.Opacity = opacities[i];
                data.DrawOrder = drawOrders[i];
                data.RenderOrder = renderOrders[i];


                // Read vertex positions only if necessary.
                if (!data.AreVertexPositionsDirty)
                {
                    continue;
                }


                // Copy vertex positions.
                fixed (Vector3* dataVertexPositions = data.VertexPositions)
                {
                    for (var v = 0; v < data.VertexPositions.Length; ++v)
                    {
                        dataVertexPositions[v].x = vertexPositions[i][v].x;
                        dataVertexPositions[v].y = vertexPositions[i][v].y;
                    }
                }
            }


            // Clear dynamic flags.
            csmResetDrawableDynamicFlags(unmanagedModel);
        }

        #endregion

        #region Extern C

        [DllImport(CubismDll.Name)]
        private static extern unsafe float* csmGetParameterValues(IntPtr model);


        [DllImport(CubismDll.Name)]
        private static extern unsafe float* csmGetPartOpacities(IntPtr model);


        // HACK Some platforms have problems with struct return types, so we use void* instead and cast in the wrapper methods.
        [DllImport(CubismDll.Name)]
        private static extern IntPtr csmGetDrawableVertexPositions(IntPtr model);

        [DllImport(CubismDll.Name)]
        private static extern unsafe byte* csmGetDrawableDynamicFlags(IntPtr model);

        [DllImport(CubismDll.Name)]
        private static extern unsafe float* csmGetDrawableOpacities(IntPtr model);

        [DllImport(CubismDll.Name)]
        private static extern unsafe int* csmGetDrawableDrawOrders(IntPtr model);

        [DllImport(CubismDll.Name)]
        private static extern unsafe int* csmGetDrawableRenderOrders(IntPtr model);


        [DllImport(CubismDll.Name)]
        private static extern void csmResetDrawableDynamicFlags(IntPtr model);

        #endregion
    }
}
