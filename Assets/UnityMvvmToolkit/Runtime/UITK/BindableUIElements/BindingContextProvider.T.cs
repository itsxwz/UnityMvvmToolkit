﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindingContextProvider<TBindingContext> : VisualElement, IBindableElement,
        IBindingContextProvider, IInitializable, IDisposable where TBindingContext : IBindingContext
    {
        private IObjectProvider _objectProvider;
        private List<IBindableElement> _bindableElements;

        private PropertyBindingData _propertyBindingData;
        private IReadOnlyProperty<TBindingContext> _bindingContextProperty;

        protected IReadOnlyList<IBindableElement> BindableElements => _bindableElements;

        public bool IsValid => !string.IsNullOrWhiteSpace(BindingContextPath);
        public IBindingContext BindingContext { get; protected set; }

        public virtual void Initialize()
        {
            if (IsValid == false)
            {
                return;
            }

            _bindableElements = this.GetBindableChilds();

            for (var i = 0; i < _bindableElements.Count; i++)
            {
                if (_bindableElements[i] is IInitializable initializable)
                {
                    initializable.Initialize();
                }
            }
        }

        public virtual void Dispose()
        {
            if (IsValid == false)
            {
                return;
            }

            foreach (var bindableElement in _bindableElements)
            {
                if (bindableElement is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _bindableElements.Clear();
        }

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (IsValid == false)
            {
                BindingContext = context;
                return;
            }

            _objectProvider = objectProvider;
            _propertyBindingData ??= BindingContextPath.ToPropertyBindingData();

            OnSetBindingContext(context, objectProvider, _propertyBindingData);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (IsValid)
            {
                OnResetBindingContext(objectProvider);
            }
        }

        protected virtual void OnSetBindingContext(IBindingContext context, IObjectProvider objectProvider,
            PropertyBindingData propertyBindingData)
        {
            _bindingContextProperty = RentBindingContextProperty(context, objectProvider, propertyBindingData);
            _bindingContextProperty.ValueChanged += OnBindingContextPropertyValueChanged;

            if (_bindingContextProperty.Value is null)
            {
                return;
            }

            BindingContext = _bindingContextProperty.Value;
            SetChildsBindingContext(BindingContext, objectProvider);
        }

        protected virtual void OnResetBindingContext(IObjectProvider objectProvider)
        {
            _bindingContextProperty.ValueChanged -= OnBindingContextPropertyValueChanged;

            objectProvider.ReturnReadOnlyProperty(_bindingContextProperty);

            BindingContext = default;
            ResetChildsBindingContext(objectProvider);

            _objectProvider = default;
            _bindingContextProperty = default;
        }

        private void OnBindingContextPropertyValueChanged(object sender, TBindingContext bindingContext)
        {
            SetChildsBindingContext(bindingContext, _objectProvider);
        }

        protected virtual IReadOnlyProperty<TBindingContext> RentBindingContextProperty(IBindingContext context,
            IObjectProvider objectProvider, PropertyBindingData propertyBindingData)
        {
            return objectProvider.RentReadOnlyProperty<TBindingContext>(context, propertyBindingData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetChildsBindingContext(IBindingContext bindingContext, IObjectProvider objectProvider)
        {
            foreach (var bindableElement in _bindableElements)
            {
                bindableElement.SetBindingContext(bindingContext, objectProvider);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetChildsBindingContext(IObjectProvider objectProvider)
        {
            foreach (var bindableElement in _bindableElements)
            {
                bindableElement.ResetBindingContext(objectProvider);
            }
        }
    }
}