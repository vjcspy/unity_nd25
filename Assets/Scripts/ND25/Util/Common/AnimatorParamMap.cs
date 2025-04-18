using System.Collections.Generic;
using UnityEngine;
namespace ND25.Util.Common
{
    /// <summary>
    ///     Lớp cơ sở ánh xạ từ enum sang Animator parameter hash.
    ///     Tối ưu hóa bằng cách cache giá trị cuối cùng và chỉ update khi thay đổi.
    /// </summary>
    public abstract class AnimatorParamMap<TEnum> where TEnum : System.Enum
    {
        private readonly Dictionary<TEnum, bool> _boolCache = new Dictionary<TEnum, bool>();
        private readonly Dictionary<TEnum, float> _floatCache = new Dictionary<TEnum, float>();
        private readonly Dictionary<TEnum, int> _intCache = new Dictionary<TEnum, int>();
        private readonly Dictionary<TEnum, int> _nameToHash = new Dictionary<TEnum, int>();

        private readonly Animator animator;

        protected AnimatorParamMap(Animator animator)
        {
            this.animator = animator;
            foreach (TEnum value in System.Enum.GetValues(enumType: typeof(TEnum)))
            {
                string name = value.ToString();
                int hash = Animator.StringToHash(name: name);
                _nameToHash[key: value] = hash;
            }
        }

        private int GetHash(TEnum param)
        {
            return _nameToHash[key: param];
        }

        public void UpdateIntParam(TEnum param, int value)
        {
            if (!_intCache.TryGetValue(key: param, value: out int last) || last != value)
            {
                animator.SetInteger(id: GetHash(param: param), value: value);
                _intCache[key: param] = value;
            }
        }

        public void UpdateFloatParam(TEnum param, float value, float epsilon = 0.001f)
        {
            if (!_floatCache.TryGetValue(key: param, value: out float last) || Mathf.Abs(f: last - value) > epsilon)
            {
                animator.SetFloat(id: GetHash(param: param), value: value);
                _floatCache[key: param] = value;
            }
        }

        public void UpdateBoolParam(TEnum param, bool value)
        {
            if (!_boolCache.TryGetValue(key: param, value: out bool last) || last != value)
            {
                animator.SetBool(id: GetHash(param: param), value: value);
                _boolCache[key: param] = value;
            }
        }
    }
}
