{{ objname | escape | underline}}

.. currentmodule:: {{ module }}

.. autoclass:: {{ objname }}
   :members: 
   :show-inheritance:
   :inherited-members:
   :exclude-members: Equals,Finalize,GetHashCode,GetType,MemberwiseClone,Overloads,ReferenceEquals,ToString,BeginScope
   
   {% block methods %}
   .. automethod:: __init__

   {% if methods %}
   =================
   Methods
   =================

   .. autosummary::
   {% for item in methods %}
      ~{{ name }}.{{ item }}
   {%- endfor %}
   {% endif %}
   {% endblock %}

   {% block attributes %}
   {% if attributes|reject("in","Equals,Finalize,GetHashCode,GetType,MemberwiseClone,Overloads,ReferenceEquals,ToString,BeginScope")|list %}
   =================
   Attributes
   =================

   .. autosummary::
   {% for item in attributes|reject("in","Equals,Finalize,GetHashCode,GetType,MemberwiseClone,Overloads,ReferenceEquals,ToString,BeginScope")|list %}
      ~{{ name }}.{{ item }}
   {%- endfor %}
   {% endif %}
   {% endblock %}