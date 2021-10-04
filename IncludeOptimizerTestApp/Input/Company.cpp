#include "Company.h"


Company::Company()
{
  m_persons.push_back({});
}

int Company::count()
{
  return m_persons.size();
}