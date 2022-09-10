# Serialization Model

Classes in this namespace approach the topic of reading and
writing XSV in a different way. Upon reading, data rows are
serialized into a typed DTO class, while upon writing instances
of that DTO class are converted to the fields of a data row.

In between the DTO instance(s) and the data row sits an adapter
class that defines the required conversions. In the initial
version of this (sub)library you need to provide not only
your DTO class but also manually set up the adapter.
