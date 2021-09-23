#pragma once

#include <memory>
class SteeringWheel;


class Car
{

private:
  std::shared_ptr<SteeringWheel> m_steeringWheel;

public:
  Car();

  
};

