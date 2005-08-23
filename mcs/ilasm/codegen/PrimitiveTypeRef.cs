//
// Mono.ILASM.PrimitiveTypeRef
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//


using System;
using System.Collections;

namespace Mono.ILASM {

        /// <summary>
        /// Reference to a primitive type, ie string, object, char
        /// </summary>
        public class PrimitiveTypeRef : ModifiableType, ITypeRef {

                private string full_name;
                private string sig_mod;
                private PEAPI.Type type;

                private bool is_resolved;
                private static Hashtable method_table = new Hashtable ();

                public PrimitiveTypeRef (PEAPI.PrimitiveType type, string full_name)
                {
                        this.type = type;
                        this.full_name = full_name;
                        sig_mod = String.Empty;
                        is_resolved = false;
                }

		public string Name {
			get { return full_name; }
		}

                public string FullName {
                        get { return full_name + sig_mod; }
                }

                public override string SigMod {
                        get { return sig_mod; }
                        set { sig_mod = value; }
                }

                public PEAPI.Type PeapiType {
                        get { return type; }
                }

                public void Resolve (CodeGen code_gen)
                {
                        if (is_resolved)
                                return;

                        // Perform all of the types modifications
                        type = Modify (code_gen, type);

                        is_resolved = true;
                }

                /// <summary>
                /// Primitive types can be created like this System.String instead
                /// of like a normal type that would be [mscorlib]System.String This
                /// method returns a proper primitive type if the supplied name is
                /// the name of a primitive type.
                /// </summary>
                public static PrimitiveTypeRef GetPrimitiveType (string full_name)
                {
                        switch (full_name) {
                        case "System.String":
                                return new PrimitiveTypeRef (PEAPI.PrimitiveType.String, full_name);
                        case "System.Object":
                                return new PrimitiveTypeRef (PEAPI.PrimitiveType.Object, full_name);
                        default:
                                return null;
                        }
                }

                public IMethodRef GetMethodRef (ITypeRef ret_type, PEAPI.CallConv call_conv,
                                string name, ITypeRef[] param)
                {
                        string key = full_name + MethodDef.CreateSignature (ret_type, name, param) + sig_mod;
                        TypeSpecMethodRef mr = method_table [key] as TypeSpecMethodRef;
                        if (mr != null)
                                return mr;

                        mr = new TypeSpecMethodRef (this, ret_type, call_conv, name, param);
                        method_table [key] = mr;
                        return mr;
                }

                public IFieldRef GetFieldRef (ITypeRef ret_type, string name)
                {
                        return new TypeSpecFieldRef (this, ret_type, name);
                }

                public IClassRef AsClassRef (CodeGen code_gen)
                {
                        /*
                        PEAPI.ClassRef class_ref = code_gen.ExternTable.GetValueClass ("corlib", FullName);
                        ExternTypeRef type_ref = new ExternTypeRef (class_ref, FullName);

                        // TODO: Need to do the rest of the conversion (in order)
                        if (IsArray)
                                type_ref.MakeArray ();

                        return type_ref;
                        */
                        throw new NotImplementedException ("This method is getting depricated.");
                }

        }

}

