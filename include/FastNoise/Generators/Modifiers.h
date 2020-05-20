#pragma once
#include "Generator.h"

namespace FastNoise
{
    class DomainScale : public virtual SingleSource<>
    {
    public:
        void SetScale( float value ) { mScale = value; };

    protected:
        float mScale = 1.0f;

        FASTNOISE_METADATA( SingleSource<> )
        
            Metadata( const char* className ) : SingleSource<>::Metadata( className )
            {
                memberVariables.emplace_back( "Scale", 1.0f, &DomainScale::SetScale );
            }
        };    
    };

    class Remap : public virtual SingleSource<>
    {
    public:
        void SetRemap( float fromMin, float fromMax, float toMin, float toMax ) { mFromMin = fromMin; mFromMax = fromMax; mToMin = toMin; mToMax = toMax; }

    protected:
        float mFromMin = -1.0f;
        float mFromMax = 1.0f;
        float mToMin = 0.0f;
        float mToMax = 1.0f;

        FASTNOISE_METADATA( SingleSource<> )
        
            Metadata( const char* className ) : SingleSource<>::Metadata( className )
            {
                memberVariables.emplace_back( "From Min", -1.0f,
                    []( Remap* p, float f )
                {
                    p->mFromMin = f;
                });
                
                memberVariables.emplace_back( "From Max", 1.0f,
                    []( Remap* p, float f )
                {
                    p->mFromMax = f;
                });
                
                memberVariables.emplace_back( "To Min", 0.0f,
                    []( Remap* p, float f )
                {
                    p->mToMin = f;
                });

                memberVariables.emplace_back( "To Max", 1.0f,
                    []( Remap* p, float f )
                {
                    p->mToMax = f;
                });
            }
        };    
    };

    class ConvertRGBA8 : public virtual SingleSource<>
    {
    public:
        void SetMinMax( float min, float max ) { mMin = min; mMax = max; }

    protected:
        float mMin = -1.0f;
        float mMax = 1.0f;

        FASTNOISE_METADATA( SingleSource<> )
        
            Metadata( const char* className ) : SingleSource<>::Metadata( className )
            {            
                memberVariables.emplace_back( "Min", -1.0f,
                    []( ConvertRGBA8* p, float f )
                {
                    p->mMin = f;
                });

                memberVariables.emplace_back( "Max", 1.0f,
                    []( ConvertRGBA8* p, float f )
                {
                    p->mMax = f;
                });
            }
        };    
    };
}
