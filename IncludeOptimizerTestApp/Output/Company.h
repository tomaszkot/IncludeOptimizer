#pragma once
#include <memory>
namespace BL{class Person;};
#include <vector>


class Company
{
std::shared_ptr<std::vector<BL::Person>> m_persons;

public:
  Company();

  int count();
};

