#include "Person.h"
#include "Company.h"


Company::Company()
{
m_persons = std::make_shared<std::vector<BL::Person>>();
m_boss = std::make_shared<BL::Person>();
  m_persons->push_back({});
  m_boss->foo();
}

int Company::count()
{
  return m_persons->size();
}