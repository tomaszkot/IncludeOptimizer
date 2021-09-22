#pragma once
#include "SteeringWheel.h"
#include <memory>

class Car
{

private:
  std::shared_ptr<SteeringWheel> m_steeringWheel;

public:
  Car();

  
};

