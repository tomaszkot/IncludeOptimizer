#pragma once
#include <vector>
#include "Person.h"


class Company
{
  std::vector<BL::Person> m_persons;
  BL::Person m_boss;

public:
  Company();

  int count();
};

