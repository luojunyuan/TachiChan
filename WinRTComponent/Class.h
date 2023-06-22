#pragma once

#include "Class.g.h"

namespace winrt::WinRTComponent::implementation
{
    struct Class : ClassT<Class>
    {
        Class() = default;

        void AFunction(hstring value);
    };
}

namespace winrt::WinRTComponent::factory_implementation
{
    struct Class : ClassT<Class, implementation::Class>
    {
    };
}
