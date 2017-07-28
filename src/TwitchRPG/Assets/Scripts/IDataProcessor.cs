using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResponseProcessor {
    /// <summary>
    /// The name which will be used to see to what response this processor will react
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Method to process the response
    /// </summary>
    /// <param name="response">The response with the data and type included</param>
    /// <returns>if true, the response may be further processed by other classes</returns>
    bool ProcessResponse(JsonRequest response);
}
