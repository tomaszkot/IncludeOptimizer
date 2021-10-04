// IncludeOptimizerTestApp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "Output/app.h"

int main()
{
    std::cout << "Hello World!\n";
    app app;
    int count = app.m_comp.count();
    std::cout << count;
    //InstType instType;
}
