#pragma once
#include "ScriptParameterValueBL.h"
#include "../Lists/EntityList.h"
#include "BaseClass.h"
#include "../../OddatBusinessLogic_Api.h"

#include <string>                          // for string

#include <Compiler/WarningsManagement.h>
INHIBIT_WARNINGS_MSVC(4251)

namespace OddatBusinessLogic
{

  namespace Entities
  {

    class ODDATBUSINESSLOGIC_API ScriptParameter : public BaseClass
    {
      std::string m_scriptParameter;
      Lists::EntityList<Entities::ScriptParameterValue> m_scriptParameterValues;
    public:
      ScriptParameter(void);
      virtual ~ScriptParameter();
      std::string getScriptParameter() const;
      void setScriptParameter(const std::string& scriptParameter);
      virtual const std::string getUniqueName() const override;
      virtual void setName(const std::string& newName) override;
      virtual const std::string getDisplayedName() const override;
      Lists::EntityList<Entities::ScriptParameterValue> getScriptParameterValues();
      void setScriptParameterValues(Lists::EntityList<Entities::ScriptParameterValue> list);

    };
  }
}

RESET_WARNINGS_MSVC
