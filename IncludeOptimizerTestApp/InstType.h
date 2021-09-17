#pragma once
#include <string>
#include <vector>
#include <memory>
#include "Model.h"

class InstType
{
  std::string m_name;
  Model model;
  //std::shared_ptr<std::vector<Model>> m_models;
  
public:

  InstType();
};

//template<typename T>
//class List
//{
//  std::vector<T> m_models;
//
//public:
//  void add(T val) { m_models.push_back(val); }
//};