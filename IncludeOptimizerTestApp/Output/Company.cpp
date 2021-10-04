#include "Person.h"
#include "Company.h"


Company::Company()
{
m_persons = std::make_shared<std::vector<BL::Person>> ();
  m_persons->push_back({});
}

int Company::count()
{
  return m_persons->size();
}