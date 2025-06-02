using Microsoft.AspNetCore.Mvc;
using UniversityApplicationSystem.Models;
using UniversityApplicationSystem.Services;

namespace UniversityApplicationSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ApplicationService _applicationService;

        public PaymentController(PaymentService paymentService, ApplicationService applicationService)
        {
            _paymentService = paymentService;
            _applicationService = applicationService;
        }

        // GET: Payment
        public IActionResult Index()
        {
            var payments = _paymentService.GetAllPayments();
            return View(payments);
        }

        // GET: Payment/Details/5
        public IActionResult Details(int id)
        {
            var payment = _paymentService.GetPaymentWithDetails(id);
            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payment/Create
        public IActionResult Create(int? applicationId)
        {
            if (applicationId.HasValue)
            {
                var application = _applicationService.GetApplication(applicationId.Value);
                if (application == null)
                {
                    TempData["Error"] = "Application not found.";
                    return RedirectToAction(nameof(Index));
                }

                var payment = new Payment
                {
                    ApplicationID = application.ApplicationID,
                    Amount = 1000.00m, // Default amount, you might want to make this configurable
                    PaymentDate = DateTime.Now,
                    Status = "Pending"
                };
                return View(payment);
            }

            return View(new Payment());
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _paymentService.CreatePayment(payment);
                    TempData["Success"] = "Payment created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating payment: {ex.Message}");
                }
            }
            return View(payment);
        }

        // GET: Payment/Edit/5
        public IActionResult Edit(int id)
        {
            var payment = _paymentService.GetPayment(id);
            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // POST: Payment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Payment payment)
        {
            if (id != payment.PaymentID)
            {
                TempData["Error"] = "Invalid payment ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the payment
                    _paymentService.UpdatePayment(payment);

                    // If payment status is completed, update the related application status to Approved
                    if (payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        var application = _applicationService.GetApplication(payment.ApplicationID);
                        if (application != null)
                        {
                            application.Status = "Approved";
                            _applicationService.UpdateApplication(application);
                        }
                    }

                    TempData["Success"] = "Payment updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating payment: {ex.Message}");
                }
            }
            return View(payment);
        }

        // POST: Payment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _paymentService.DeletePayment(id);
                TempData["Success"] = "Payment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting payment: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Payment/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status, string transactionId = null)
        {
            try
            {
                _paymentService.UpdatePaymentStatus(id, status, transactionId);
                if (status == "Completed")
                {
                    var payment = _paymentService.GetPayment(id);
                    if (payment != null)
                    {
                        _applicationService.UpdateApplicationStatus(payment.ApplicationID, "Approved", User.Identity?.Name ?? "System");
                    }
                }
                TempData["Success"] = "Payment status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating payment status: {ex.Message}";
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}