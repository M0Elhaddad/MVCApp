using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IEmployeeRepository _employeeRepo;
        //private readonly IDepartmentRepository _departmentRepo;

        public EmployeeController(IMapper mapper,IUnitOfWork unitOfWork/*IEmployeeRepository employeeRepo,IDepartmentRepository departmentRepo*/)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            //_employeeRepo = employeeRepo;
            //_departmentRepo = departmentRepo;
        }
        public IActionResult Index(string SearchInp)
        {
            var employees =Enumerable.Empty<Employee>();

            if(string.IsNullOrEmpty(SearchInp))
                 employees = _unitOfWork.EmployeeRepository.GetAll();
            else
                 employees = _unitOfWork.EmployeeRepository.SearchByName(SearchInp.ToLower());

            var mappedEmp = _mapper.Map<IEnumerable<Employee>,IEnumerable<EmployeeViewModel>>(employees);

            return View(mappedEmp);
            
        }

        public IActionResult Create()
        {
            //ViewData["Departments"] = _departmentRepo.GetAll();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid)
            {
                employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");
                var mappedEmp = _mapper.Map<EmployeeViewModel,Employee>(employeeVM);
                _unitOfWork.EmployeeRepository.Add(mappedEmp);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(employeeVM);
        }

        public IActionResult Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();//400

            var employee = _unitOfWork.EmployeeRepository.GetById(id.Value);

            if (employee is null)
                return NotFound();//404
            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(employee);

            return View(ViewName, mappedEmp);
        }

        public IActionResult Edit(int? id)
        {
            //ViewData["Departments"] = _departmentRepo.GetAll();

            return Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");

                    employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");

                    var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                    
                    
                    _unitOfWork.EmployeeRepository.Update(mappedEmp);
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);

                }
            }
            return View(employeeVM);
        }
        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();
            try
            {
                

                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                _unitOfWork.EmployeeRepository.Delete(mappedEmp);
                int count = _unitOfWork.Complete();
                if (count > 0) 
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVM);
            }
        }
    }
}
