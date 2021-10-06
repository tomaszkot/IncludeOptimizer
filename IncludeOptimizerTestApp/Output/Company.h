#pragma once
#include <memory>
namespace BL{class Person;};
#include <vector>


class Company
{
std::shared_ptr<std::vector<BL::Person>> m_persons;
  std::shared_ptr<BL::Person> m_boss;

public:
  Company();

  int count();
};

