﻿using MvcTemplate.Components.Mvc;
using MvcTemplate.Data.Migrations;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace MvcTemplate.Resources.Tests
{
    public class ResourcesTests
    {
        [Fact]
        public void Resources_HasAllPageTitles()
        {
            XElement sitemap = XDocument.Load("../../../../../src/MvcTemplate.Web/mvc.sitemap").Element("siteMap");

            foreach (SiteMapNode node in Flatten(sitemap.Elements()).Where(node => node.Action != null))
            {
                String path = $"{node.Area}/{node.Controller}/{node.Action}";

                Assert.True(!String.IsNullOrEmpty(Resource.ForPage(path)), $"'{path}' page does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllSiteMapTitles()
        {
            XElement sitemap = XDocument.Load("../../../../../src/MvcTemplate.Web/mvc.sitemap").Element("siteMap");

            foreach (String path in Flatten(sitemap.Elements()).Select(node => $"{node.Area}/{node.Controller}/{node.Action}"))
                Assert.True(!String.IsNullOrEmpty(Resource.ForSiteMap(path)), $"'{path}' page does not have a sitemap title.");
        }

        [Fact]
        public void Resources_HasAllPermissionAreaTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (Permission permission in context.Set<Permission>().Where(permission => permission.Area != null))
                Assert.True(!String.IsNullOrEmpty(Resource.ForArea(permission.Area!)), $"'{permission.Area}' area does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionControllerTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (String path in context.Set<Permission>().Select(permission => $"{permission.Area}/{permission.Controller}").Distinct())
                Assert.True(!String.IsNullOrEmpty(Resource.ForController(path)), $"'{path}' permission does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionActionTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (String name in context.Set<Permission>().Select(permission => permission.Action).Distinct())
                Assert.True(!String.IsNullOrEmpty(Resource.ForAction(name)), $"'{name}' action does not have a title.");
        }

        private List<SiteMapNode> Flatten(IEnumerable<XElement> elements, SiteMapNode? parent = null)
        {
            List<SiteMapNode> list = new List<SiteMapNode>();
            foreach (XElement element in elements)
            {
                SiteMapNode node = new SiteMapNode
                {
                    Action = (String)element.Attribute("action"),
                    Area = (String)element.Attribute("area") ?? parent?.Area,
                    Controller = (String)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null)
                };

                list.Add(node);
                list.AddRange(Flatten(element.Elements(), node));
            }

            return list;
        }
    }
}
