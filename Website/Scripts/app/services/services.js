﻿// Define you service here. Services can be added to same module as 'main' or a seperate module can be created.

var reportServices = angular.module('report.services', ['ngResource']);     //Define the services module

var reportService = function ($resource, $rootScope) {
    var
        clusterStatusProxy = $resource('/api/report', { id: '@id' }, {
            postMemberLeaveCluster: { url: '/api/report/memberleavecluster', method: 'POST', params: { host: '@host', port: '@port', protocol: '@protocol', system: '@system' }, isArray: false },
            postDownMember: { url: '/api/report/downmember', method: 'POST', params: { host: '@host', port: '@port', protocol: '@protocol', system: '@system' }, isArray: false }
        }),

        memberLeaveCluster = function (member) {
            return clusterStatusProxy.postMemberLeaveCluster({ host: member.Host, port: member.Port, protocol: member.Protocol, system: member.System }).$promise;
        },

        downMember = function (member) {
            return clusterStatusProxy.postDownMember({ host: member.Host, port: member.Port, protocol: member.Protocol, system: member.System }).$promise;
        };

    return {
        memberLeaveCluster: memberLeaveCluster,
        downMember: downMember
    };
};
reportServices.factory('ReportService', reportService);

var reportApiService = function ($http, $rootScope) {
    var
        getClusterState = function (url) {
            return $http.get(url);
        },

        memberLeaveCluster = function (member) {
            return clusterStatusProxy.postMemberLeaveCluster({ host: member.Host, port: member.Port, protocol: member.Protocol, system: member.System }).$promise;
        },

        downMember = function (member) {
            return clusterStatusProxy.postDownMember({ host: member.Host, port: member.Port, protocol: member.Protocol, system: member.System }).$promise;
        };

    return {
        getClusterState: getClusterState,
        memberLeaveCluster: memberLeaveCluster,
        downMember: downMember
    };
};
reportServices.factory('ReportApiService', reportApiService);