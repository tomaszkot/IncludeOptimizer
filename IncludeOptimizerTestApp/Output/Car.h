#pragma once
#include <memory>
#include "SteeringWheel.h"

class Car
{
  std::shared_ptr<SteeringWheel> m_steeringWheel;
};

