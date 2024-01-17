using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Dynatrace.OpenTelemetry;
using Dynatrace.OpenTelemetry.Instrumentation.AwsLambda;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AWSLambda;
using OpenTelemetry.Trace;
using System;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace myfunctionsdk;

public class Function
{
        private static readonly TracerProvider TracerProvider;


        static Function()
        {
            DynatraceSetup.InitializeLogging();
            TracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddDynatrace()
                .AddAWSLambdaConfigurations(c => c.DisableAwsXRayContextExtraction = true)
                .AddAWSInstrumentation(c => c.SuppressDownstreamInstrumentation = true)
                .AddDynatraceAwsSdkInjection()
                .Build();
        }

        public Task FunctionHandler(object input, ILambdaContext context)
        {
            var propagationContext = AwsLambdaHelpers.ExtractPropagationContext(context);
            return AWSLambdaWrapper.TraceAsync(TracerProvider, FunctionHandlerInternalAsync, input, context, propagationContext.ActivityContext);
        }


        private Task FunctionHandlerInternalAsync(object input, ILambdaContext context)
        {
            return Task.CompletedTask;
        }
}
