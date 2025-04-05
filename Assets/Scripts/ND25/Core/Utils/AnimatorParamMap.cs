using System;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Core.Utils
{
    /// <summary>
    ///     Lớp cơ sở ánh xạ từ enum sang Animator parameter hash.
    ///     Không dùng singleton, mỗi subclass cần được khởi tạo riêng.
    /// </summary>
    public abstract class AnimatorParamMap<TEnum> where TEnum : Enum
    {
        readonly Dictionary<string, int> _nameToHash = new Dictionary<string, int>();

        protected AnimatorParamMap()
        {
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                string name = value.ToString();
                int hash = Animator.StringToHash(name);
                _nameToHash[name] = hash;
            }
        }

        /// <summary>
        ///     Truy cập bằng enum an toàn.
        /// </summary>
        public int Get(TEnum param)
        {
            return _nameToHash[param.ToString()];
        }

        /// <summary>
        ///     Truy cập bằng string. Có fallback nếu không tìm thấy.
        /// </summary>
        public int Get(string name)
        {
            // return _nameToHash.TryGetValue(name, out int hash) ? hash : Animator.StringToHash(name); // fallback để không crash
            return _nameToHash[name];
        }
    }
}
