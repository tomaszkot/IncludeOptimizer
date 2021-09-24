#include <memory>
namespace Cars{class SteeringWheel;};
#pragma once

namespace Cars
{
  class Car
  {

  private:
    std::shared_ptr<Cars::SteeringWheel> m_steeringWheel;

  public:
    Car();


  };
}
