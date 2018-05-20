#r "Newtonsoft.Json"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static IActionResult Run(HttpRequest req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string dni = req.Query["dni"];
	string category = req.Query["category"];
	
    string requestBody = new StreamReader(req.Body).ReadToEnd();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    dni = dni ?? data?.dni;
	
	category = category ?? data?.category;
	
	if (!ValidateDNI(dni)) return new BadRequestObjectResult("Please validate that the dni parameter has the right format");
	if (!ValidateCat(category)) return new BadRequestObjectResult("Please validate that the category has the right format");
	
	

    return (ActionResult)new OkObjectResult(GetCUIL(dni, category));
}

public static bool ValidateDNI(string DNI)
{
	int dni = 0;
	
	if (DNI.Length != 8) return false;

            try
            { dni = Convert.ToInt32(DNI); }
            catch (Exception)
            { return false; }

            return true;
	
	return true;
}

public static bool ValidateCat(string cat)
{
	char tempChar = new char();
	try
	{ tempChar = cat[0]; }
	catch (Exception) { return false; }
	
	switch (tempChar)
	{
		case 'm':
		case 'f':
		case 'c':
			return true;
		default:
			return false;
	}
}

public static string GetCUIL(string DNI, string category)
{
	int sum = 0;
	int checkIdentifier = 0;
	string CUIL = string.Empty;

	List<int> digits = new List<int>();

	switch (category[0])
	{
		case 'm':
			digits = 20.ToString().Select(t => int.Parse(t.ToString())).ToList();
			CUIL = "20-";
			break;
		case 'f':
			digits = 27.ToString().Select(t => int.Parse(t.ToString())).ToList();
			CUIL = "27-";
			break;
		case 'c':
			digits = 30.ToString().Select(t => int.Parse(t.ToString())).ToList();
			CUIL = "30-";
			break;
		default:
			break;
	}

	digits.AddRange(DNI.ToString().Select(t => int.Parse(t.ToString())).ToList());
	CUIL = string.Concat(CUIL, DNI, "-");

	int index = 5;
	foreach (var digit in digits)
	{
		sum += digit * index;
		index--;
		if (index == 1) index = 7;
	}

	checkIdentifier = 11 - (sum % 11);

	CUIL = string.Concat(CUIL, checkIdentifier.ToString());

	return CUIL;
}