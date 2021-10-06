#include "Company.h"


Company::Company()
{
  m_persons.push_back({});
  m_boss.foo();
}

int Company::count()
{
  return m_persons.size();
}